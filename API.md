# 内部接口设计

MVP 不提供 HTTP API。本文件定义应用层与基础设施之间的 C# 内部契约，具体签名可在实现时按 Unity 版本和测试需求微调，但职责边界不得随意合并。

## IContentRepository

```csharp
Task<ContentLoadResult> LoadExperimentAsync(string experimentId, CancellationToken cancellationToken);
```

职责：读取、解析并校验实验配置。失败时返回结构化问题列表，不返回部分可运行配置。

当前 Unity 实现为 `ChemistryLab.Infrastructure.Content.ExperimentContentJsonRepository`，负责将 UTF-8 JSON 映射为内容模型并调用校验器。发布路径必须传入审核门禁，拒绝 `reviewStatus: "draft"` 的内容。

## ExperimentSessionFactory

```csharp
ExperimentSessionStartResult StartFromJson(string json, bool requireApproved);
```

当前实现为 `ChemistryLab.Application.Sessions.ExperimentSessionFactory`。它协调内容加载、审核门禁和流程创建：只有内容仓库返回有效内容后，才将配置步骤转换为 `ExperimentDefinition` 并启动 `ExperimentWorkflow`。失败时返回原始的内容校验问题，且不会创建部分运行中的会话。

`ExperimentSessionStartResult` 包含成功标记、已验证内容、已启动工作流或问题列表。它是当前主菜单与未来 Unity 场景绑定之间的最小应用层入口；测试中仅可传入显式标记为 `SYNTHETIC_TEST_DATA` 的合成 JSON，不能将其作为已审核教学参数。

## IWorkflowRunner

```csharp
ExperimentState Start(ExperimentDefinition definition);
CommandResult Execute(ExperimentCommand command);
ExperimentState Pause();
ExperimentState Resume();
ExperimentState Reset();
```

职责：维护实验状态与步骤转换。命令失败不得产生部分状态更新。

## IRuleEvaluator

```csharp
RuleEvaluationResult Evaluate(
    ExperimentState state,
    ExperimentCommand command,
    ExperimentDefinition definition);
```

职责：验证操作顺序、参数范围、前置条件和配置规则，返回稳定错误码、学生提示和恢复建议。

当前核心实现 `ChemistryLab.Core.Instrument.InstrumentController` 接受 `InstrumentAction` 并返回 `InstrumentTransitionResult`。它提供供电、泵和等离子体的最小教学状态，不依赖 Unity UI，也不包含真实仪器型号、数值参数或 SOP；后续由内容配置将教师审核规则接入该边界。

## ICalculationService

```csharp
CalibrationResult FitCalibration(IReadOnlyList<CalibrationPoint> points);
ConcentrationResult CalculateConcentration(
    MeasurementResult sample,
    CalibrationResult calibration,
    SampleCalculationRule rule);
```

职责：执行确定性计算并报告输入不足、退化拟合、非有限数值和超出适用范围等问题。

当前 Unity 实现为 `ChemistryLab.Core.Calculation.LinearCalibrationService`，入口为：

```csharp
LinearCalibrationResult Fit(IReadOnlyList<CalibrationPoint> points);
```

它以最小二乘法拟合 `response = slope * concentration + intercept`，并返回 R²、点数和稳定的 `CalibrationErrorCode`。`SampleConcentrationCalculator` 使用成功标定按 `(sampleResponse - intercept) / slope` 反算浓度，并返回稳定的 `SampleConcentrationErrorCode`。目前测试点为合成数值；标准溶液浓度、响应、接受阈值、稀释、空白扣除和其他样品换算规则仍待教师审核后通过内容配置接入。

## IRecordStore

```csharp
Task<SaveRecordResult> SaveAsync(ExperimentRecord record, CancellationToken cancellationToken);
Task<LoadRecordResult> LoadAsync(string recordId, CancellationToken cancellationToken);
Task<IReadOnlyList<RecordSummary>> ListAsync(CancellationToken cancellationToken);
```

职责：在本地用户数据目录保存和读取记录。路径由实现控制，调用者不能传入任意文件路径。

当前 Unity 实现 `ChemistryLab.Infrastructure.Records.ExperimentRecordJsonStore` 保存和读取最小记录快照。记录 ID 使用 GUID，加载入口拒绝非 GUID 值；后续 UI 会以 `Application.persistentDataPath/records/` 创建该存储实例。

## ChemistryLabDemoView

`ChemistryLab.UI.ChemistryLabDemoView` 是当前 `SampleScene` 的原型 UI 入口。它在运行时创建 Canvas、状态文本和操作按钮，并调用应用已有的 `ExperimentWorkflow` 与 `InstrumentController`。界面中的内容明确标记为合成测试数据；正式 UI 仍需接入教师审核的内容配置和本地化资源。

## 核心数据约定

- `ExperimentDefinition`：已通过结构、版本、引用和审核状态校验的不可变配置。
- `ExperimentState`：当前会话、步骤、仪器状态、参数和测量结果的不可变快照。
- `ExperimentCommand`：学生的一次意图，例如设置参数、启动泵或提交测量。
- `CommandResult`：成功状态、错误码、用户提示和新状态。
- `CalibrationResult`：斜率、截距、拟合指标、点数、有效范围和诊断信息。
- `ExperimentRecord`：内容版本、操作事件、输入、计算结果、时间和完成状态。

所有跨模块 DTO 使用稳定 ID，不依赖 Unity 场景对象引用。JSON 字段采用 `camelCase`，C# 类型和属性采用 `PascalCase`。

## 错误码约定

```text
CONTENT_*      配置加载与校验
WORKFLOW_*     状态和步骤转换
RULE_*         操作顺序与参数规则
CALCULATION_*  拟合与浓度计算
RECORD_*       本地记录读写
```

错误码用于测试和日志，面向学生的文本由内容配置或本地化资源提供。
