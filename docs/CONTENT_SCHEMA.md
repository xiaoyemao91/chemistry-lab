# 教学内容配置约定

本文件定义 MVP JSON 配置的设计边界。实现时应补充正式 JSON Schema 和自动校验测试。

## 文件位置

```text
Assets/StreamingAssets/content/
```

每个实验使用独立目录或主配置文件，并通过稳定 ID 引用步骤、参数和规则。运行时只读取这些内容，不原地修改。

## 顶层字段

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `schemaVersion` | string | 配置结构版本，例如 `1.0`。 |
| `contentVersion` | string | 教学内容版本。 |
| `experimentId` | string | 稳定的小写 kebab-case ID。 |
| `displayName` | string | 学生看到的实验名称。 |
| `reviewStatus` | string | `draft` 或 `approved`。 |
| `sourceReference` | string | SOP 或教师资料版本引用。 |
| `steps` | array | 有序实验步骤。 |
| `parameters` | array | 参数定义、单位和约束。 |
| `rules` | array | 操作顺序与错误规则。 |
| `calibration` | object | 标准曲线与拟合约定。 |
| `sampleCalculation` | object | 样品换算约定。 |

## 占位示例

下例只展示结构，所有科学值均为占位符，不能作为真实实验参数：

```json
{
  "schemaVersion": "1.0",
  "contentVersion": "0.1.0-draft",
  "experimentId": "fe-measurement",
  "displayName": "Fe 测定",
  "reviewStatus": "draft",
  "sourceReference": "PENDING_TEACHER_REVIEW",
  "steps": [
    {
      "stepId": "power-on-check",
      "title": "开机与状态检查",
      "allowedCommands": ["confirm-status"]
    }
  ],
  "parameters": [],
  "rules": [],
  "calibration": {
    "model": "PENDING_TEACHER_REVIEW"
  },
  "sampleCalculation": {
    "formulaId": "PENDING_TEACHER_REVIEW"
  }
}
```

## 校验要求

- 拒绝未知或不兼容的 `schemaVersion`。
- 拒绝缺失字段、重复 ID、无效引用和未知命令。
- 所有数值必须为有限值，并具有明确单位和允许范围。
- 步骤图必须存在入口和可达完成状态，不允许无意循环。
- 发布构建只加载 `reviewStatus: "approved"` 的内容。
- `sourceReference` 不能为空，科学公式必须可追溯。
- 配置规模应设上限，避免异常文件造成内存或界面问题。

## 版本策略

- 仅内容值变化：更新 `contentVersion`。
- 向后兼容的字段新增：增加次版本并提供默认处理。
- 破坏性结构变化：增加 `schemaVersion`，旧版本明确拒绝或通过独立迁移工具转换。

