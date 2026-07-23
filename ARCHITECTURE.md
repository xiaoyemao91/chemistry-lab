# 系统架构

## 1. 架构目标

采用高内聚、低耦合的分层结构，使教学流程、科学计算、Unity 表现层和本地文件系统能够分别开发与测试。MVP 只实现当前需求需要的接口，不预建后端或插件体系。

## 2. 分层结构

```text
UI / Unity Scene
        ↓
Application（用例编排）
        ↓
Core（流程、规则、计算、领域模型）
        ↓
Infrastructure（JSON 内容与本地记录）
```

- `Core`：不依赖 Unity UI，包含实验状态、步骤、参数、规则、线性拟合和浓度计算。
- `Application`：组织开始实验、提交操作、执行测量、生成结果和保存记录等用例。
- `Infrastructure`：实现 JSON 加载、版本校验、文件路径和记录持久化。
- `UI`：MonoBehaviour、页面控制器、输入组件、状态显示、动画和用户提示。

依赖方向保持向内。`Core` 不引用 `UI` 或具体文件实现。

## 3. 核心模块

| 模块 | 职责 | 不负责 |
| --- | --- | --- |
| Workflow | 状态机、步骤推进、暂停、重置、完成条件 | 渲染页面、直接读写文件 |
| Instrument | 教学状态、可执行操作、状态变化事件 | 真实硬件驱动、高精度物理计算 |
| Method | 参数定义、单位、默认值和合法性 | UI 输入控件 |
| Calculation | 线性拟合、拟合质量、浓度和换算 | 决定教学步骤 |
| Content | 配置模型、版本与引用完整性 | 修改运行中的领域状态 |
| Record | 实验记录模型、保存与读取 | 学生账号或云同步 |
| UI | 页面展示、输入采集和反馈 | 保存科学规则或计算公式 |

## 4. 运行数据流

```text
StreamingAssets/content/*.json
  → ContentRepository 解析与校验
  → ExperimentDefinition
  → WorkflowRunner 建立运行状态
  → 学生操作 / 参数输入
  → RuleEvaluator 校验
  → CalculationService 计算
  → UI 展示
  → RecordStore 保存到 Application.persistentDataPath
```

配置校验必须先于实验开始。科学参数未经教师审核时，配置必须标记为 `draft`，发行构建不得加载草稿内容。

## 5. 状态设计

建议的顶层实验状态：

```text
NotStarted → Running ↔ Paused → Completed
                  ↓
                Reset
```

当前步骤由稳定的 `stepId` 标识。学生操作转换为命令，由规则引擎根据当前状态和配置判定接受或拒绝。UI 不直接改变流程状态。

## 6. 持久化

### 教学内容

- 路径：`Assets/StreamingAssets/content/`
- 格式：UTF-8 JSON
- 要求：包含配置版本、内容版本、审核状态和稳定 ID
- 读取：只读；应用运行时不修改教师内容

### 实验记录

- 根目录：`Application.persistentDataPath/records/`
- 格式：一个实验一次 JSON 记录
- 写入：先写临时文件，校验后原子替换目标文件
- 标识：使用应用生成的记录 ID，不使用学生姓名作为文件名

MVP 不使用数据库。出现查询、统计或多用户需求后再评估 SQLite 或服务端。

## 7. 错误处理

- 配置错误：阻止实验启动，显示用户可理解信息，并记录开发诊断详情。
- 操作错误：不改变状态，返回错误码、提示和恢复建议。
- 计算错误：区分输入不足、退化数据、数值无效和规则不支持。
- 保存错误：保留当前实验状态，允许重试，不宣称已保存。

## 8. 测试架构

- EditMode：流程状态机、规则、线性拟合、浓度计算、配置校验。
- PlayMode：页面与状态绑定、完整 Fe 流程、重置、重复操作和记录保存。
- 手工验收：离线启动、不同分辨率、异常配置、文件不可写和教师参考数据。

## 9. 目录映射

```text
Assets/
├── Scenes/
├── Scripts/
│   ├── Core/
│   ├── Application/
│   ├── Infrastructure/
│   └── UI/
├── StreamingAssets/content/
├── Content/
├── Art/
├── Audio/
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

当第一批代码出现时，为 `Core`、`Application`、`Infrastructure`、`UI` 和测试分别建立 Assembly Definition，明确依赖边界。

