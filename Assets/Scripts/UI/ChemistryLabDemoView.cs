using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ChemistryLab.Application.Sessions;
using ChemistryLab.Core.Calculation;
using ChemistryLab.Core.Instrument;
using ChemistryLab.Core.Records;
using ChemistryLab.Core.Workflow;
using ChemistryLab.Infrastructure.Content;
using ChemistryLab.Infrastructure.Records;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChemistryLab.UI
{
    public sealed class ChemistryLabDemoView : MonoBehaviour
    {
        private readonly InstrumentController instrument = new InstrumentController();
        private readonly ExperimentContentJsonRepository contentRepository = new ExperimentContentJsonRepository(new ChemistryLab.Core.Content.ExperimentContentValidator());
        private ExperimentRecordJsonStore recordStore;
        private readonly Dictionary<string, InputField> parameterInputs = new Dictionary<string, InputField>();
        private Text statusText;
        private Transform panelTransform;
        private ExperimentWorkflow workflow;
        private ChemistryLab.Core.Content.ExperimentContentDefinition content;
        private Guid recordId;
        private LinearCalibrationResult calibrationResult;

        private void Start()
        {
            recordStore = new ExperimentRecordJsonStore(Path.Combine(UnityEngine.Application.persistentDataPath, "records"));
            CreateInterface();
            UpdateStatus("合成测试实验已准备；请先启动实验，再按仪器顺序操作。", Color.white);
        }

        private void CreateInterface()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            var canvasObject = new GameObject("ChemistryLabCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var panel = CreatePanel(canvasObject.transform, new Color(0.05f, 0.08f, 0.12f, 0.96f));
            panelTransform = panel.transform;
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CreateLabel(panel.transform, "ICP-OES 虚拟仿真实验", new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.96f), 30, Color.white);
            CreateLabel(panel.transform, "当前版本：合成测试数据原型 | 科学参数待教师审核", new Vector2(0.05f, 0.77f), new Vector2(0.95f, 0.84f), 15, new Color(0.75f, 0.82f, 0.9f));
            statusText = CreateLabel(panel.transform, string.Empty, new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.74f), 18, Color.white);

            CreateButton(panel.transform, "开始实验", new Vector2(0.05f, 0.49f), new Vector2(0.30f, 0.59f), StartExperiment);
            CreateButton(panel.transform, "开机", new Vector2(0.35f, 0.49f), new Vector2(0.50f, 0.59f), () => ExecuteInstrument(InstrumentAction.PowerOn));
            CreateButton(panel.transform, "启动泵", new Vector2(0.55f, 0.49f), new Vector2(0.70f, 0.59f), () => ExecuteInstrument(InstrumentAction.StartPump));
            CreateButton(panel.transform, "点燃等离子体", new Vector2(0.75f, 0.49f), new Vector2(0.95f, 0.59f), () => ExecuteInstrument(InstrumentAction.IgnitePlasma));
            CreateButton(panel.transform, "完成当前步骤", new Vector2(0.05f, 0.35f), new Vector2(0.30f, 0.45f), CompleteStep);
            CreateButton(panel.transform, "熄灭等离子体", new Vector2(0.35f, 0.35f), new Vector2(0.55f, 0.45f), () => ExecuteInstrument(InstrumentAction.ExtinguishPlasma));
            CreateButton(panel.transform, "停止泵", new Vector2(0.60f, 0.35f), new Vector2(0.75f, 0.45f), () => ExecuteInstrument(InstrumentAction.StopPump));
            CreateButton(panel.transform, "关机", new Vector2(0.80f, 0.35f), new Vector2(0.95f, 0.45f), () => ExecuteInstrument(InstrumentAction.PowerOff));
        }

        private void StartExperiment()
        {
            var contentPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "content", "fe-measurement.json");
            try
            {
                var contentJson = File.ReadAllText(contentPath);
                var sessionResult = new ExperimentSessionFactory(contentRepository).StartFromJson(contentJson, false);
                if (!sessionResult.IsSuccess)
                {
                    UpdateStatus("内容无法启动：" + sessionResult.Issues[0].Code, Color.yellow);
                    return;
                }

                content = sessionResult.Content;
                recordId = Guid.NewGuid();
                CreateParameterInputs(content.Parameters);
                workflow = sessionResult.Workflow;
                UpdateStatus("实验已开始：" + workflow.State.CurrentStepId + "，参数项：" + content.Parameters.Count, Color.green);
            }
            catch (IOException)
            {
                UpdateStatus("内容文件读取失败：" + contentPath, Color.yellow);
            }
        }

        private void CompleteStep()
        {
            if (workflow == null)
            {
                UpdateStatus("请先开始实验。", Color.yellow);
                return;
            }

            if (workflow.State.CurrentStepId == "parameter-setup" && !ValidateParameters(out var validationMessage))
            {
                UpdateStatus(validationMessage, Color.yellow);
                return;
            }

            if (workflow.State.CurrentStepId == "calibration")
            {
                var points = new List<CalibrationPoint>();
                foreach (var point in content.CalibrationPoints) points.Add(new CalibrationPoint(point.Concentration, point.Response));
                calibrationResult = new LinearCalibrationService().Fit(points);
                if (!calibrationResult.IsSuccess)
                {
                    UpdateStatus("标定失败：" + calibrationResult.ErrorCode, Color.yellow);
                    return;
                }
            }

            var result = workflow.CompleteCurrentStep();
            if (!result.IsSuccess)
            {
                UpdateStatus("流程错误：" + result.ErrorCode, Color.yellow);
                return;
            }

            if (workflow.State.Status == ExperimentStatus.Completed)
            {
                var saveResult = recordStore.Save(new ExperimentRecord(
                    recordId,
                    content.ExperimentId,
                    content.ContentVersion,
                    workflow.State.Status.ToString(),
                    workflow.State.CurrentStepId,
                    DateTime.UtcNow));
                UpdateStatus(
                    saveResult.IsSuccess ? "实验完成，R²=" + calibrationResult.DeterminationCoefficient.ToString("0.000") + "，记录已保存：" + recordId : "实验完成但记录保存失败：" + saveResult.ErrorCode,
                    saveResult.IsSuccess ? Color.green : Color.yellow);
                return;
            }

            UpdateStatus("流程状态：" + workflow.State.Status + "，当前步骤：" + (workflow.State.CurrentStepId ?? "无"), Color.green);
        }

        private void ExecuteInstrument(InstrumentAction action)
        {
            var result = instrument.Execute(action);
            var state = result.State;
            var message = result.IsSuccess
                ? "仪器状态：供电=" + state.IsPoweredOn + "，泵=" + state.IsPumpRunning + "，等离子体=" + state.IsPlasmaIgnited
                : "仪器操作被拒绝：" + result.ErrorCode;
            UpdateStatus(message, result.IsSuccess ? Color.green : Color.yellow);
        }

        private void UpdateStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }
        }

        private void CreateParameterInputs(IReadOnlyList<ChemistryLab.Core.Content.ExperimentParameterDefinition> parameters)
        {
            if (parameterInputs.Count > 0 || parameters == null) return;

            CreateLabel(panelTransform, "分析参数（合成测试数据，可编辑）", new Vector2(0.05f, 0.25f), new Vector2(0.95f, 0.31f), 13, new Color(0.8f, 0.86f, 0.92f));
            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                var minimum = 0.05f + index * 0.30f;
                var maximum = minimum + 0.27f;
                CreateLabel(panelTransform, parameter.DisplayName + " (" + parameter.Unit + ")", new Vector2(minimum, 0.18f), new Vector2(maximum, 0.24f), 11, Color.white);
                var input = CreateInputField(panelTransform, parameter.DefaultValue.ToString(CultureInfo.InvariantCulture), new Vector2(minimum, 0.10f), new Vector2(maximum, 0.17f));
                parameterInputs.Add(parameter.ParameterId, input);
            }
        }

        private bool ValidateParameters(out string message)
        {
            foreach (var parameter in content.Parameters)
            {
                if (!parameterInputs.TryGetValue(parameter.ParameterId, out var input)
                    || !double.TryParse(input.text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    message = "参数输入无效：" + parameter.DisplayName;
                    return false;
                }

                if (value < parameter.Minimum || value > parameter.Maximum)
                {
                    message = parameter.DisplayName + " 超出范围：" + parameter.Minimum + " - " + parameter.Maximum + " " + parameter.Unit;
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        private static GameObject CreatePanel(Transform parent, Color color)
        {
            var panel = new GameObject("ExperimentPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Text CreateLabel(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, int size, Color color)
        {
            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(parent, false);
            var rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = size;
            label.color = color;
            label.alignment = TextAnchor.MiddleLeft;
            return label;
        }

        private static void CreateButton(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(text, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = new Color(0.12f, 0.32f, 0.5f, 1f);
            buttonObject.GetComponent<Button>().onClick.AddListener(action);
            var label = CreateLabel(buttonObject.transform, text, Vector2.zero, Vector2.one, 16, Color.white);
            label.alignment = TextAnchor.MiddleCenter;
        }

        private static InputField CreateInputField(Transform parent, string value, Vector2 anchorMin, Vector2 anchorMax)
        {
            var inputObject = new GameObject("ParameterInput", typeof(RectTransform), typeof(Image), typeof(InputField));
            inputObject.transform.SetParent(parent, false);
            var rect = inputObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            inputObject.GetComponent<Image>().color = new Color(0.92f, 0.95f, 0.98f, 1f);
            var input = inputObject.GetComponent<InputField>();
            var text = CreateLabel(inputObject.transform, value, Vector2.zero, Vector2.one, 12, Color.black);
            text.alignment = TextAnchor.MiddleCenter;
            input.textComponent = text;
            input.text = value;
            return input;
        }
    }
}
