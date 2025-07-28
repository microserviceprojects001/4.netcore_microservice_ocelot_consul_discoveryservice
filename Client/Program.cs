// Program.cs
using DnsClient;
using System.Net;

var dnsEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8600); // Consul DNS 默认端口
var client = new LookupClient(dnsEndpoint);

// 查询服务
var result = await client.QueryAsync("servicename.service.consul", QueryType.SRV);

// 处理结果
foreach (var record in result.Answers.SrvRecords())
{
    Console.WriteLine($"服务: {record.Target}:{record.Port}");
    Console.WriteLine($"优先级: {record.Priority}, 权重: {record.Weight}");
}