# GenerateChart MCP Server

HTTP MCP 图表生成服务。工具生成 PNG 图片并返回签名图片 URL

## 本地运行

```bash
dotnet run --project GenerateChartMcpServer
```

服务默认监听 `http://localhost:52345`，MCP 端点由 ASP.NET Core MCP 映射提供，图片通过 `/images/{filename}` 返回，健康检查为 `/health`。

## 本地联调

```bash
npx @modelcontextprotocol/inspector http://localhost:52345/mcp
```

如果开启了 `ApiKey`，请求 MCP 端点时需要带 `X-API-Key` / `Authorization` 头，兼容 `http://localhost:52345/mcp?api_key=your-secret-key-here` 形式的查询参数鉴权。

## HTTP 发布准备

1. 用 [GenerateChartMcpServer/Dockerfile](GenerateChartMcpServer/Dockerfile) 构建容器镜像。
2. 把 `ImageBaseUrl` 配成外网可访问的图片地址前缀，例如 `https://your-host.example.com/images`。
3. 在生产环境设置 `ApiKey` 和 `AllowedOrigins`。