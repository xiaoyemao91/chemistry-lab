# ICP-OES 虚拟仿真教学软件

面向学生的 Windows 单机版 ICP-OES 实验仿真软件。项目采用 Unity 2022.3 LTS 与 C#，通过 2D/照片式界面模拟实验流程，首个 MVP 聚焦 Fe 测定闭环。

## 当前状态

项目处于初始化阶段。Unity 工程已创建，科学参数、仪器操作细节和参考结果仍需教师确认。未经教师确认的数据只能作为显式标注的占位内容，不得作为真实实验规则。

## MVP 流程

```text
开机与状态检查
→ 设置 ICP-OES 分析参数
→ Fe 标准溶液测量
→ 建立标定曲线
→ 样品测量
→ 计算 Fe 浓度
→ 保存实验记录
```

## 技术范围

- Unity `2022.3.62f3c1` + C#
- 2D Built-In Render Pipeline
- Windows 离线运行
- 教学内容由版本化 JSON 配置维护
- 实验记录保存在 `Application.persistentDataPath`
- MVP 不包含生成式 AI、三维模型、硬件控制、后端服务或数据库

## 打开项目

1. 使用 Unity Hub 打开本项目目录。
2. 确认编辑器版本为 `2022.3.62f3c1`。
3. 从 `Assets/Scenes/` 打开当前开发场景。
4. 使用 Unity Test Runner 运行 EditMode 和 PlayMode 测试。

## 文档入口

- [需求](REQUIREMENTS.md)
- [架构](ARCHITECTURE.md)
- [路线图](ROADMAP.md)
- [内部接口](API.md)
- [编码规范](CODING_RULES.md)
- [教师资料清单](docs/TEACHER_INPUT_CHECKLIST.md)
- [内容配置约定](docs/CONTENT_SCHEMA.md)
- [MVP 验收](docs/MVP_ACCEPTANCE_TEST.md)

## 数据与隐私

仓库不得包含学生个人信息、实验室负责人信息、仪器序列号、内部账号、未授权照片或其他实验室敏感资料。提交素材前必须确认授权和脱敏状态。

