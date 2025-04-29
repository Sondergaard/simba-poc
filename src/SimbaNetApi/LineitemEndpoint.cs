using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SimbaNetApi;

public static class LineitemEndpoint
{
    private const string ConnectionString = "DSN=Databricks";
    private const string Query = @"SELECT 
        l_orderkey AS OrderKey,
        l_partkey AS PartKey,
        l_suppkey AS SuppKey,
        l_linenumber AS LineNumber,
        l_quantity AS Quantity,
        l_extendedprice AS ExtendedPrice,
        l_discount AS Discount,
        l_tax AS Tax,
        l_returnflag AS ReturnFlag,
        l_linestatus AS LineStatus,
        l_shipdate AS ShipDate,
        l_commitdate AS CommitDate,
        l_receiptdate AS ReceiptDate,
        l_shipinstruct AS ShipInstruct,
        l_shipmode AS ShipMode,
        l_comment AS Comment    
     FROM samples.tpch.lineitem";

    public static IResult GetAllLineitems()
    {
        try
        {

            return Results.Stream(async stream => {

                var lineBreak = Encoding.UTF8.GetBytes("\n");
                var seperator = Encoding.UTF8.GetBytes(",");

                using var connection = new OdbcConnection(ConnectionString);
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = Query;
                cmd.CommandType = CommandType.Text;
                using var reader = cmd.ExecuteReader(CommandBehavior.Default);

                await stream.WriteAsync(Encoding.UTF8.GetBytes("[")); // Start of JSON array
                
                while (await reader.ReadAsync())
                {
                    var lineitem = new Lineitem(
                        OrderKey: reader.GetInt64(0),
                        PartKey: reader.GetInt64(1),
                        SuppKey: reader.GetInt64(2),
                        LineNumber: reader.GetInt32(3),
                        Quantity: reader.GetDecimal(4),
                        ExtendedPrice: reader.GetDecimal(5),
                        Discount: reader.GetDecimal(6),
                        Tax: reader.GetDecimal(7),
                        ReturnFlag: reader.GetString(8),
                        LineStatus: reader.GetString(9),
                        ShipDate: reader.GetDateTime(10),
                        CommitDate: reader.GetDateTime(11),
                        ReceiptDate: reader.GetDateTime(12),
                        ShipInstruct: reader.GetString(13),
                        ShipMode: reader.GetString(14),
                        Comment: reader.GetString(15));

                    var json = JsonSerializer.SerializeToUtf8Bytes(lineitem);
                    await stream.WriteAsync(json);
                    await stream.WriteAsync(seperator);
                    await stream.WriteAsync(lineBreak);
                }
                
                await stream.WriteAsync(lineBreak);
                await stream.WriteAsync(Encoding.UTF8.GetBytes("]"));  // End of JSON array

            }, 
                contentType: "application/json",
                fileDownloadName: "lineitems.json");
        }
        catch (Exception e)
        {
            ProblemDetails problem = new()
            {
                Status = 500,
                Detail = e.Message,
                Title = "An unexpected error occurred"
            };

            return Results.Problem(problem);
        }
    }

    public record Lineitem(long OrderKey, long PartKey, long SuppKey, int LineNumber, Decimal Quantity,
        Decimal ExtendedPrice, Decimal Discount, Decimal Tax, string ReturnFlag, string LineStatus,
        DateTime ShipDate, DateTime CommitDate, DateTime ReceiptDate, string ShipInstruct,
        string ShipMode, string Comment);    
}
