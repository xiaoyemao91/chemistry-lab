using System.IO;
using ChemistryLab.Application.Sessions;
using ChemistryLab.Core.Instrument;
using ChemistryLab.Core.Workflow;
using ChemistryLab.Infrastructure.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChemistryLab.UI
{
    public sealed class ChemistryLabDemoView : MonoBehaviour
    {
        private readonly InstrumentController instrument = new InstrumentController();
        private readonly ExperimentContentJsonRepository contentRepository = new ExperimentContentJsonRepository(new ChemistryLab.Core.Content.ExperimentContentValidator());
        private Text statusText;
        private ExperimentWorkflow workflow;

        private void Start()
        {
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

                workflow = sessionResult.Workflow;
                UpdateStatus("实验已开始：" + workflow.State.CurrentStepId, Color.green);
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

            var result = workflow.CompleteCurrentStep();
            UpdateStatus(result.IsSuccess ? "流程状态：" + workflow.State.Status + "，当前步骤：" + (workflow.State.CurrentStepId ?? "无") : "流程错误：" + result.ErrorCode, result.IsSuccess ? Color.green : Color.yellow);
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
    }
}
