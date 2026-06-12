# GenerateChart MCP Server

基于 ScottPlot 的 HTTP MCP 图表生成服务，根据输入数据生成静态 PNG 图片并返回签名图片 URL。

## 支持的图表类型

| 工具名称 | 图表类型 | 说明 |
|---------|---------|------|
| `GenerateBarChart` | 柱状图 | 垂直柱状图，支持多系列 |
| `GenerateHorizontalBarChart` | 条形图 | 水平条形图，支持多系列 |
| `GenerateStackedBarChart` | 堆叠条形图 | 堆叠条形图，支持多系列 |
| `GenerateLineChart` | 折线图 | 折线图，支持多系列 |
| `GeneratePieChart` | 饼图 | 饼图 |
| `GenerateBubbleChart` | 气泡图 | 气泡散点图，支持多系列 |

## 本地运行

```bash
dotnet run --project GenerateChartMcpServer
```

服务默认监听 `http://localhost:52345`，MCP 端点由 ASP.NET Core MCP 映射提供，图片通过 `/images/{filename}` 返回，健康检查为 `/health`。

## 本地联调

```bash
npx @modelcontextprotocol/inspector http://localhost:52345
```

如果开启了 `ApiKey`，请求 MCP 端点时需要带 `X-API-Key` / `Authorization` 头，兼容 `http://localhost:52345/?api_key=your-secret-key-here` 形式的查询参数鉴权。

## 接入配置

```json
{
  "servers": {
    "GenerateChartMcpServer": {
      "type": "http",
      "url": "http://localhost:52345",
      "headers": {
        "X-API-Key": "your-secret-key-here"
      }
    }
  }
}
```

如果本地不需要鉴权，可以把 `ApiKey` 设为空，并移除 `headers`。

## MCP 服务配置项

| 配置 | 说明 |
|------|------|
| `Urls` | ASP.NET Core 监听地址 |
| `ApiKey` | MCP 端点访问密钥；为空时不校验 |
| `AllowedOrigins` | 允许访问 MCP 端点的浏览器 `Origin` 白名单 |
| `ImageBaseUrl` | 工具返回给客户端的图片 URL 前缀，生产环境必须配置成公网地址 |
| `ImageStoragePath` | 图片落盘目录，目录下按 `yy[a-l]` 分月归档，例如 `24a`、`24b` |
| `ImageExpireMonths` | 图片文件过期时间，单位月；整数，`0` 表示永不过期，默认 `1` |
| `SignExpireSeconds` | 图片签名有效期，单位秒 |

## 平台支持

- `win-x64` / `win-arm64`
- `osx-arm64`
- `linux-x64` / `linux-arm64` / `linux-musl-x64`


## Docker 容器

### 运行容器

```bash
docker run --rm -p 52345:8080 \
  -e ApiKey=your-secret-key-here \
  -e ImageBaseUrl=https://your-host.example.com \
  -e AllowedOrigins__0=https://your-client.example.com \
  ghcr.io/sangyuxiaowu/generatechartmcpserver:latest
```

### 自行构建

```bash
docker build -t generatechartmcpserver:latest .
```

## 自行部署

1. 下载并解压目标制品到服务器，例如 `/opt/generatechartmcpserver`。
2. 修改 `generatechartmcpserver.env.example`，写入真实 `ApiKey`、`ImageBaseUrl`、`AllowedOrigins` 和 `FontName`。
3. 复制 `GenerateChartMcpServer.service` 到 `/etc/systemd/system/`，并按实际安装路径调整 `WorkingDirectory` 和 `ExecStart`。
4. 执行 `systemctl daemon-reload && systemctl enable --now GenerateChartMcpServer`。
5. 配置反向代理（如 Nginx 或 Apache）和 HTTPS，将外部请求转发到服务监听端口。

本地生成 Linux x64 单文件制品示例：

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

## More information

.NET MCP servers use the [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol) C# SDK. For more information about MCP:

- [Official Documentation](https://modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)
