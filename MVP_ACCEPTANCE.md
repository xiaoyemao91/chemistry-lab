# ICP-OES 虚拟仿真教学软件 MVP 验收记录

## 已验证

- Unity 2022.3.62f3c1 项目可打开并运行 `SampleScene`。
- EditMode 回归测试：31 个测试通过。
- PlayMode 回归测试：1 个测试通过。
- Unity 编辑器手工闭环：启动实验、暂停、恢复、参数校验、仪器顺序操作、标定拟合、样品反算、记录保存和 GUID 读取。
- 合成闭环结果：浓度 `3.000`，`R²=1.000`。
- Windows 64 位构建成功：`D:/Codex-Workplace/artifacts/ChemistryLabMVP/chemistry-lab.exe`。
- 构建产物离线启动检查通过，进程成功存活 5 秒。

## 明确限制

- 当前所有参数、标定点和样品响应都标记为 `SYNTHETIC_TEST_DATA`。
- 当前版本不是已通过教师审核的真实 ICP-OES SOP，也不连接真实仪器。
- 教师提供并审核真实型号、SOP、参数范围、标准溶液数据和计算规则后，才能替换合成内容用于正式教学。
- `ProjectSettings/ProjectSettings.asset` 中的既有 Unity 自动改动未纳入本次提交。
