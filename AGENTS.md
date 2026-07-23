# Project Instructions

## Scope

本项目是 Windows 单机版 ICP-OES 教学仿真软件，使用 Unity 2022.3 LTS、C#、2D Built-In Render Pipeline 和本地 JSON 配置。

## Required workflow

1. 开发前阅读 `REQUIREMENTS.md`、`ARCHITECTURE.md` 和 `CODING_RULES.md`。
2. 一次只实现一个可验证模块。
3. 先确定成功标准，再设计、开发、测试、修复和更新文档。
4. 计算、规则和流程逻辑必须可脱离 Unity UI 测试。
5. 修改代码后运行与范围匹配的 EditMode 或 PlayMode 测试。

## Scientific integrity

- 不猜测 ICP-OES 型号、SOP、波长、功率、气流、浓度、单位、公式或参考结果。
- 未经教师确认的数据必须标记为 `draft` 或 `placeholder`。
- 教师确认的科学内容必须保留来源、版本和单位信息。
- 发行构建不得加载草稿科学内容。

## Scope limits

MVP 不增加生成式 AI、三维仿真、硬件接口、后端、数据库、Docker 或云服务，除非用户明确扩展范围。

## Git and privacy

- 不自动创建分支、提交或推送。
- 提交前检查照片、人员信息、仪器编号、账号和内部数据是否已授权并脱敏。
- 保护任务开始前已有的用户改动。

