# MCP 配置建议

MVP 开发只依赖当前主机已提供或可直接使用的基础能力：

- Filesystem：读取和修改项目文件
- Terminal：运行 Unity、测试和辅助脚本
- Git：本地版本控制
- GitHub：查看私有仓库、Issue 和 Pull Request；推送前必须单独确认

当前不配置 Docker、Postgres、Redis、数据库 MCP、云端 AI 或独立浏览器自动化。项目本身不应依赖 MCP 才能运行。

MCP 配置通常属于开发者本机环境，不把令牌、登录状态或用户专属绝对路径提交到仓库。未来只有在明确出现新需求时再增加连接器，并记录用途、权限范围和替代方案。

