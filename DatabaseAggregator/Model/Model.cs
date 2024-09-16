using Aspose.Cells;
using HtmlAgilityPack;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DatabaseAggregator.Model
{
    public class Model 
    {
        private readonly HttpClient httpClient;
        private ObservableCollection<Database> Databases { get; set; } 
        private readonly string puthResource = Path.Combine(Environment.CurrentDirectory, "Resource");
        private static readonly string API_KEY = "6cbe6e35-f52e-410f-a627-444352adf9c3";
        private const string HTTP_REQUEST_FSTEC = "https://bdu.fstec.ru/files/documents/vullist.xlsx";
        private const string HTTP_REQUEST_NVD = "https://services.nvd.nist.gov/rest/json/cves/2.0/?resultsPerPage=1000&startIndex=0";
        private const string HTTP_REQUEST_JVN = "https://jvndb.jvn.jp/search/index.php?mode=_vulnerability_search_IA_VulnSearch&lang=en&keyword=&dateLastPublishedFromYear=2023&dateLastPublishedFromMonth=11&datePublicFromYear=2023&datePublicFromMonth=11&skey=d6";

        

        public Model()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };
            httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            if (!Directory.Exists(puthResource))
                Directory.CreateDirectory(puthResource);
        }

        private async Task<ObservableCollection<Vulnerability>> UpdateByInstallationLink()
        {
            if (!File.Exists(Path.Combine(puthResource, "FSTEC.xlsx")))
                await DownloadFileFromLink();

            ObservableCollection<Vulnerability> vulnerabilities = new();

            try
            {
                using Workbook workbook = new(Path.Combine(Environment.CurrentDirectory, "FSTEC.xlsx"));
                using Worksheet worksheet = workbook.Worksheets[0];

                var numberRows = worksheet.Cells.MaxDataRow;
                var numberColumn = worksheet.Cells.MaxDataColumn;

                for (int rowIterator = 3; rowIterator < numberRows; rowIterator++)
                {
                    for (int columnIterator = 0; columnIterator < numberColumn; columnIterator++)
                    {
                        Vulnerability vulnerability = new() { ParameterAndDescription = new() };
                        for (int lineIterator = 0; lineIterator < numberColumn; lineIterator++)
                        {
                            if (lineIterator is 18)
                                continue;
                            vulnerability.ParameterAndDescription.Add((string)worksheet.Cells[2, lineIterator].Value, Convert.ToString(worksheet.Cells[rowIterator, lineIterator].Value));
                        }
                        vulnerability.Identifier = Convert.ToString(worksheet.Cells[rowIterator, 0].Value);
                        vulnerabilities.Add(vulnerability);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return vulnerabilities;
        }
        private async Task DownloadFileFromLink()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, HTTP_REQUEST_FSTEC);
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using Stream stream = await response.Content.ReadAsStreamAsync();

                    Process[] processList;
                    processList = Process.GetProcessesByName("EXCEL");
                    foreach (var process in processList) { process.Kill(); }

                    await stream.CopyToAsync(new FileStream(Path.Combine(Environment.CurrentDirectory, "FSTEC.xlsx"), FileMode.Create));
                }
                else
                {
                    throw new Exception(GetExceptionMessage(response.StatusCode));
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async Task<string> GetJsonStringByApi()
        {
            string json = string.Empty;
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, HTTP_REQUEST_NVD);
                request.Headers.Add("User-Agent", $"{API_KEY}");
                using HttpResponseMessage response = httpClient.Send(request);

                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(GetExceptionMessage(response.StatusCode));
                }
            }
            catch (Exception ex)
            {
            }
            return json;
        }
        public async Task<ObservableCollection<Vulnerability>> UpdateByApiRequest()
        {
            var json = JToken.Parse(await GetJsonStringByApi());
            var trips = json["vulnerabilities"];
            ObservableCollection<Vulnerability> vulnerabilities = new();
            foreach (JToken trip in trips)
            {
                Vulnerability vulnerability = new();

                var cve = trip["cve"];
                var id = cve["id"];
                var references = cve["references"];
                var metrics = cve["metrics"];
                var cvssData = (metrics["cvssMetricV2"]?.First["cvssData"] ?? metrics["cvssMetricV31"]?.First["cvssData"]) ?? null;

                if (cvssData is not null)
                {
                    var vectorString = cvssData["vectorString"] ?? "Парметр не задан";
                    var baseScore = cvssData["baseScore"] ?? "Парметр не задан";
                    var version = cvssData["version"] ?? "Парметр не задан";
                    var accessComplexity = cvssData["accessComplexity"] ?? "Парметр не задан";
                    var confidentialityImpact = cvssData["confidentialityImpact"] ?? "Парметр не задан";
                    var integrityImpact = cvssData["integrityImpact"] ?? "Парметр не задан";
                    var availabilityImpact = cvssData["availabilityImpact"] ?? "Парметр не задан";

                    string links = string.Empty;
                    foreach (var reference in references)
                        links += $"{(string)reference["url"]}\n";

                    vulnerability.ParameterAndDescription = new()
                                {
                                    { "Последнее изменение", (string)cve["lastModified"] },
                                    { "Описание", (string)cve["descriptions"].First["value"] },
                                    { "Идентификатор источника", (string)cve["sourceIdentifier"] },
                                    { "Базовый балл" , (string)baseScore},
                                    {"Версия метрики", (string)version },
                                    { "Вектор", (string)vectorString },
                                    {"Дата опубликоваия", (string) cve["published"]},
                                    {"Статус" , (string)cve["vulnStatus"]},
                                    {"Сложность доступа", (string )accessComplexity},
                                    {"Воздействие на конфиденциальность", (string )confidentialityImpact},
                                    {"Воздействие на целостность", (string)integrityImpact},
                                    {"Воздействие на доступность", (string)availabilityImpact},
                                    {"Ссылки", links }
                                };
                }

                vulnerability.Identifier = (string)id;
                vulnerabilities.Add(vulnerability);
            }
            return vulnerabilities;
        }
        public static ObservableCollection<Vulnerability> UpdateByPageParsing()
        {
            HtmlWeb web = new();
            List<string> refVul = new();
            ObservableCollection<Vulnerability> vulnerabilities = new();

            var htmlDoc = web.Load(HTTP_REQUEST_JVN);
            var numberVul = htmlDoc.DocumentNode.SelectNodes("//table[@class='result_class']/tr").Count;
            for (int i = 2; i <= numberVul; i++)
            {
                var refer = htmlDoc.DocumentNode.SelectSingleNode($"//table[@class='result_class']/tr[{i}]/td[1]/a[1]").Attributes["href"].Value;
                refVul.Add("https://jvndb.jvn.jp" + refer);
            }
            for (int i = 0; i < refVul.Count; i++)
            {
                Vulnerability vulnerability = new();

                var htmlDocument = web.Load(refVul[i]);

                vulnerability.Identifier = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='news-list']/table[1]/tr[2]").InnerText.Replace("\n", string.Empty);
                vulnerability.ParameterAndDescription = new()
                    {
                        { "Описание", htmlDocument.DocumentNode.SelectSingleNode("//div[@id='news-list']/table[1]/tr[5]").InnerText.Replace("\n", string.Empty) },
                    };
                vulnerabilities.Add(vulnerability);
            }
            return vulnerabilities;
        }
        private static string GetExceptionMessage(HttpStatusCode statusCode)
        {
            if (statusCode is HttpStatusCode.Forbidden)
                return "Сервер отказывается выполнить запрос";
            else if (statusCode is HttpStatusCode.Gone)
                return "Ресурс больше недоступен";
            else if (statusCode is HttpStatusCode.InternalServerError)
                return "На сервере произошла общая ошибка";
            else if (statusCode is HttpStatusCode.ServiceUnavailable)
                return "Сервер временно недосутпен";
            else
                return "Непредвиденная ошибка";
        }
        public async Task CreatDatabase()
        {
            try
            {
                Databases = new ObservableCollection<Database>()
            {
                new("FSTEC", await UpdateByInstallationLink()),
                new("NVD",  await UpdateByApiRequest()),
                new("JVN",  UpdateByPageParsing())
            };
            }
            catch (Exception ex)
            {

            }
        }
        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new() { Filter = "Json files (*.json)|*.json|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() is true)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(Databases, options);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
    }
    public class Vulnerability : INotifyPropertyChanged
    {
        private string identifier;
        public string Identifier
        {
            get => identifier;
            set
            {
                identifier = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<string, string> parameterAndDescription;
        public Dictionary<string, string> ParameterAndDescription
        {
            get => parameterAndDescription;
            set
            {
                parameterAndDescription = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class Database
    {
        public string Name { get; set; }
        public ObservableCollection<Vulnerability> Vulnerabilities { get; set; }

        public Database(string name, ObservableCollection<Vulnerability> vulnerabilities)
        {
            Name = name;
            Vulnerabilities = vulnerabilities;
        }
    }
}