using Grpc.Net.Client;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net;
using System.Text;
using System.Net.Mime;
using Newtonsoft.Json;
using Google.Protobuf;

namespace VectorDbConsoleApp
{
    internal class Program
    {
        private static string PROXY = "";
        private static string QDRANT_ADDRESS = "http://localhost:6334";
        private static string QDRANT_API_KEY = "";
        static async Task Main(string[] args)
        {
            var client = GetClient();
            var health = await client.HealthAsync();
            var init = health.IsInitialized();
            //bool isCollectionExist = await client.CollectionExistsAsync("test_collection");
            //if (isCollectionExist)
            //{
            //    await client.DeleteCollectionAsync("test_collection");
            //}
            //await client.CreateCollectionAsync("test_collection", new VectorParams { Size = 384, Distance = Distance.Cosine });

            await client.RecreateCollectionAsync("test_collection", new VectorParams { Size = 384, Distance = Distance.Cosine });

            await client.UpsertAsync(collectionName: "test_collection", points: CreateEmbededVectorDataList());

            var search = await client.SearchAsync("test_collection", Embed("komboda column gibi componentlerde"));
            Console.WriteLine(search.OrderByDescending(x => x.Score).First().Payload.First());
        }

        private static List<PointStruct> CreateEmbededVectorDataList()
        {
            return new List<PointStruct>
            {
                new()
                {
                    Id = 1,
                        Vectors = Embed("UTextEdit: UControl\r\nEditlenebilen kontrollerin base class'ını temsil eder.\r\n\r\n\r\nCharacterSet: CharacterSets\r\nİzin verilen karakter setini tutar.\r\n\r\nExcludedCharacters : string\r\nÇıkarılmak istenen karakterleri tutar.\r\n\r\nIncludedCharacters : string\r\nİzin verilmek istenen karakterleri tutar.\r\n\r\nIsSpaceAllowed : boolean\r\nBoşluk karakterine izin verilme durumunu tutar.\r\n\r\nIsCharacterSetExtra1Allowed : boolean\r\nBoşluk karakterine izin verilme durumunu tutar.\r\n\r\nIsSpellCheckEnabled : boolean\r\nYazım denetimi aktif/pasif durumunu tutar.\r\n\r\nCaseStyle: CaseStyles\r\nİnput içerisine girilen değerleri belirli stillere dönüştürmek için kullanılır.\r\n\r\nText : string\r\nİnput içerisine girilen değeri verir.\r\n\r\nDefaultValue : string\r\nİnput içerisine verilen varsayılan değeri verir.\r\n\r\nRegularExpressionPattern : RegularExpressionPatternTypes\r\nRegex kullanılmak istendiği durumda bu özellik kullanılır.\r\n\r\nIsPasteAllowed : boolean\r\nclipboard dan paste özelliğine izin verilme durumunu tutar.\r\n\r\nRegularExpression : string\r\nManuel regex yazılması için kullanılan özelliktir.\r\n\r\nIsReplacingLocalSpecificsAllowed : boolean\r\nTürkçe karakterler İngilizce karakterlerle değiştirilebilip değiştirilemeyeceğini belirler\r\n\r\nIsDialogEnabled : boolean\r\nInput içerisinden diyalog penceresinin açılıp açılamayacağını belirler.\r\n\r\nDialogScreenCode : string\r\nDiyalog tuşu komut nesnesidir. IsDialogEnabled = true olması durumunda kontrolün sağında görülen butona basılınca çalışacak komuttur.\r\n\r\nDialogScreenResultPropertyName : string\r\nTextBox içinden diyalog olarak açılacak ekrandan dönen değer(ler)in alınacağı propertynin adıdır.\r\n\r\nIsDialogScreenReadAutomatically : boolean\r\nTextBox içinden diyalog olarak açılacak ekran açılışta okuma işlemini otomatik yapacak mı?").ToArray(),
                        Payload = {
                            ["ComponentName"] = "UTextEdit"
                        }
                },
                new()
                {
                    Id = 2,
                        Vectors = Embed("URowExpression\r\nRow expresionlara ait class. Datagrid, combobox gibi komponentlerde rowa ait expression verilerini tutar.\r\n\r\n\r\nValue: string\r\nExpression bilgisini tutar.\r\n\r\nColumnName: string\r\nKolon adını tutar.\r\n\r\nCondition: LogicalConditions\r\nExpression koşulunu tutar. Varsayılan değeri None.\r\n\r\nMatchedCellType: GridExpressionBasedCellTypes\r\nEşleşen hücrenin tipini tutar. Varsayılan değeri None.\r\n\r\nMatchedCellColor: UIRowColors\r\nEşleşen hücrenin arkaplan rengini tutar.\r\n\r\nMatchedCellForegroundColor: UIColors\r\nEşleşen hücrenin yazı rengini tutar.\r\n\r\nMatchedRowColor: UIRowColors\r\nEşleşen satırın rengini tutar.\r\n\r\nMatchedCellIcon: UIIcons\r\nEşleşen hücrenin icon bilgisini tutar.").ToArray(),
                        Payload = {
                            ["ComponentName"] = "URowExpression"
                        }
                },
                new()
                {
                    Id = 3,
                        Vectors = Embed("UColumnExpression\r\nColumn expresionlara ait class. Datagrid, combobox gibi komponentlerde columna ait expression verilerini tutar.\r\n\r\n\r\nValue: string\r\nExpression bilgisini tutar.\r\n\r\nColumnName: string\r\nKolon adını tutar.\r\n\r\nCondition: LogicalConditions\r\nExpression koşulunu tutar. Varsayılan değeri None.\r\n\r\nMatchedCellType: GridExpressionBasedCellTypes\r\nEşleşen hücrenin tipini tutar. Varsayılan değeri None.\r\n\r\nMatchedCellColor: UIRowColors\r\nEşleşen hücrenin arkaplan rengini tutar.\r\n\r\nMatchedCellForegroundColor: UIColors\r\nEşleşen hücrenin yazı rengini tutar.\r\n\r\nMatchedRowColor: UIRowColors\r\nEşleşen satırın rengini tutar.\r\n\r\nMatchedCellIcon: UIIcons\r\nEşleşen hücrenin icon bilgisini tutar.").ToArray(),
                        Payload = {
                            ["ComponentName"] = "UColumnExpression"
                        }
                },
            };
        }

        private static QdrantClient GetClientWithProxy()
        {
            var proxy = new WebProxy(PROXY) { UseDefaultCredentials = true };
            var httpHandler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            var httpClient = new HttpClient(httpHandler);
            httpClient.DefaultRequestHeaders.Add("api-key", QDRANT_API_KEY);

            var grpcChannel = GrpcChannel.ForAddress(
                QDRANT_ADDRESS,
                new GrpcChannelOptions { HttpClient = httpClient }
            );

            var grpcClient = new QdrantGrpcClient(grpcChannel);
            return new QdrantClient(grpcClient);
        }

        /// <summary>
        /// doesn't work without httpclient proxy...
        /// </summary>
        /// <returns></returns>
        private static QdrantClient GetClient()
        {
            var channel = QdrantChannel.ForAddress(QDRANT_ADDRESS, new ClientConfiguration
            {
                //ApiKey = QDRANT_API_KEY,
            });
            var grpcClient = new QdrantGrpcClient(channel);
            return new QdrantClient(grpcClient);
        }

        private static float[] Embed(string text)
        {
            string[] requestPayload = new string[] { text };
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("http://localhost:1453/embed"),
                    Method = HttpMethod.Post,
                };
                request.Content = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, MediaTypeNames.Application.Json);
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                var httpResponse = client.Send(request);
                httpResponse.EnsureSuccessStatusCode();
                var responsePayload = httpResponse.Content.ReadAsStringAsync().Result;
                //Console.WriteLine(responsePayload);
                return JsonConvert.DeserializeObject<VectorResult>(responsePayload).Vectors.First();
            }
        }
    }

    file class VectorResult
    {
        public float[][] Vectors { get; set; }
    }
}
