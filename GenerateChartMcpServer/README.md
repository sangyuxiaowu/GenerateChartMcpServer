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

## 数据格式

### 柱状图 / 条形图 / 堆叠条形图 / 折线图

```json
{
  "xLabels": "[\"一月\",\"二月\",\"三月\"]",
  "seriesData": "[{\"name\":\"销量\",\"values\":[120,200,150]}]"
}
```

### 饼图

```json
{
  "labels": "[\"苹果\",\"香蕉\",\"橘子\"]",
  "values": "[30,20,50]"
}
```

### 气泡图

```json
{
  "seriesData": "[{\"name\":\"城市\",\"x\":[1,2,3],\"y\":[10,20,30],\"size\":[40,50,60]}]"
}
```

## 本地开发

```json
{
  "servers": {
    "GenerateChartMcpServer": {
      "type": "http",
      "url": "http://localhost:52345/mcp",
      "headers": {
        "X-API-Key": "your-secret-key-here"
      }
    }
  }
}
```

如果本地不需要鉴权，可以把 `ApiKey` 设为空，并移除 `headers`。

## 配置项

| 配置 | 说明 |
|------|------|
| `Urls` | ASP.NET Core 监听地址 |
| `ApiKey` | MCP 端点访问密钥；为空时不校验 |
| `AllowedOrigins` | 允许访问 MCP 端点的浏览器 `Origin` 白名单 |
| `ImageBaseUrl` | 工具返回给客户端的图片 URL 前缀，生产环境必须配置成公网地址 |
| `ImageStoragePath` | 图片落盘目录 |
| `SignExpireSeconds` | 图片签名有效期，单位秒 |

## 平台支持

- `win-x64` / `win-arm64`
- `osx-arm64`
- `linux-x64` / `linux-arm64` / `linux-musl-x64`

## 测试 HTTP MCP 服务

```bash
dotnet run
npx @modelcontextprotocol/inspector http://localhost:52345/mcp
```

## 发布 HTTP 服务

### 构建容器

```bash
docker build -t ghcr.io/sangyuxiaowu/generatechartmcpserver:0.1.0-beta .
```

### 运行容器

```bash
docker run --rm -p 52345:8080 \
  -e ApiKey=your-secret-key-here \
  -e ImageBaseUrl=https://your-host.example.com/images \
  -e AllowedOrigins__0=https://your-client.example.com \
  ghcr.io/sangyuxiaowu/generatechartmcpserver:latest
```

### 自行部署

1. 发布 ASP.NET Core 应用到服务器，确保服务器环境满足 .NET 运行时要求。
2. 配置反向代理（如 Nginx 或 Apache）将外部请求转发到 ASP.NET Core 应用。
3. 配置 HTTPS 和 CORS，确保安全访问 MCP 端点和图片资源。

发布示例，Linux x64：

```bash
otnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

## More information

.NET MCP servers use the [ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol) C# SDK. For more information about MCP:

- [Official Documentation](https://modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

Refer to the VS Code or Visual Studio documentation for more information on configuring and using MCP servers:

- [Use MCP servers in VS Code (Preview)](https://code.visualstudio.com/docs/copilot/chat/mcp-servers)
- [Use MCP servers in Visual Studio (Preview)](https://learn.microsoft.com/visualstudio/ide/mcp-servers)
