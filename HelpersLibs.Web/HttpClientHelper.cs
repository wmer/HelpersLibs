using HelpersLib.Strings;
using HelpersLibs.Web.Events;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System.IO;
using System.IO.Pipes;

namespace HelpersLibs.Web;
public class HttpClientHelper : IDisposable {
    private HttpClient _client;
    private string _mediaType;

    public event RequisitionEventHandler Requisition;
    public event ResponseEventHandler Response;

    public HttpClientHelper(HttpClient client) {
        _client = client;
    }



    public HttpClientHelper(string baseAdress) {
        var handler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (requestMessage, certificate, chain, policyErrors) => true
        };

        _client = new HttpClient(handler) {
            Timeout = TimeSpan.FromMinutes(30)
        };

        if (!string.IsNullOrEmpty(baseAdress)) {
            _client.BaseAddress = new Uri(baseAdress);
        }

        _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
    }

    public HttpClientHelper() : this("") {}

    public HttpClientHelper SetBaseAddress(string baseAdress) {
        _client.BaseAddress = new Uri(baseAdress);  

        return this;
    }

    public HttpClientHelper AddcontentType(string mediaTypeHeader = "application/json", string charset = "utf-8", bool withoutalidation = false) {
        _mediaType = mediaTypeHeader;

        if (withoutalidation) {
            return AddCustomHeaders("Content-Type", $"{mediaTypeHeader}; charset={charset}");
        }

        var mediaType = new MediaTypeWithQualityHeaderValue(mediaTypeHeader);
        mediaType.CharSet = charset;
        _client.DefaultRequestHeaders.Accept.Add(mediaType);


        return this;
    }

    public HttpClientHelper AddBearerAuthentication(string token, bool convertToBase64 = false) {
        var _token = token;
        if (convertToBase64) {
            _token = StringHelper.Base64Encode(token);
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return this;
    }

    public HttpClientHelper AddBasicAuthentication(string userName, string password) {
        var authentication = StringHelper.Base64Encode($"{userName}:{password}");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authentication);
        return this;
    }

    public HttpClientHelper AddBasicAuthentication(string token) {
        var authentication = StringHelper.Base64Encode(token);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authentication);
        return this;
    }

    public HttpClientHelper AddCustomHeaders(string key, string value) {
        if (_client.DefaultRequestHeaders.TryGetValues(key, out IEnumerable<string>? values) && values?.Count() > 0) {
            var ddd = values.ToList();
            var dd = "";
        } else {
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }
        return this;
    }

    public async Task<(T result, string statusCode, string message, HttpResponseHeaders cabecario)> GetAsync<T>(string endPoint, CancellationToken cancelationToken = default) {
        var callstr = $"{_client.BaseAddress}{endPoint}";

        using var response = await _client.GetAsync(callstr, cancelationToken);
        OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));
        return await DeserializeResponseAsync<T>(response);
    }



    public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> GetAsync<T, TResult>(string endPoint, T obj) {
        var callstr = $"{_client.BaseAddress}{endPoint}";

        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri(callstr);
        request.Content = ObjectToHttpContent(obj, "GET", endPoint);

        var response = await _client.SendAsync(request);


        OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));
        return await DeserializeResponseAsync<TResult>(response);

    }

    public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PostAsync<T, TResult>(string endPoint, T obj) {
        var ddd = $"{_client.BaseAddress}{endPoint}";
        using var response = await _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj, "POST", endPoint));
        return await DeserializeResponseAsync<TResult>(response);
    }


    public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PostMultipartFormDataContentAsync<TResult>(string endPoint, MultipartFormDataContent obj) {
        using var response = await _client.PostAsync($"{_client.BaseAddress}{endPoint}", obj);
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        OnRequisition(this, new RequisitionEventArgs(endPoint, "POST", json));

        return await DeserializeResponseAsync<TResult>(response);
    }

    public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PostWithFormDataAsync<TResult>(string endPoint, FormUrlEncodedContent obj) {
        using var response = await _client.PostAsync($"{_client.BaseAddress}{endPoint}", obj);
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        OnRequisition(this, new RequisitionEventArgs(endPoint, "POST", json));

        return await DeserializeResponseAsync<TResult>(response);
    }

    public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PutAsync<T, TResult>(string endPoint, T obj) {
        using var response = await _client.PutAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj, "PUT", endPoint));
        return await DeserializeResponseAsync<TResult>(response);
    }

    public async Task<(bool result, string message)> DeleteAsync(string endPoint) {
        using var response = await _client.DeleteAsync($"{_client.BaseAddress}{endPoint}");
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode) {
            return (true, responseContent);
        } else {
            return (false, responseContent);
        }
    }

    public async Task<byte[]> GetFileAsync(string endPoint, CancellationToken cancelationToken = default) {
        var callstr = $"{_client.BaseAddress}{endPoint}";
        OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));

        var fileStream = await _client.GetStreamAsync(callstr, cancelationToken);
        byte[] buffer = new byte[16 * 1024];
        using MemoryStream ms = new MemoryStream();
        int read;
        while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
            ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
    }

    public async Task DownloadAsync(string endPoint, string fileToSave, CancellationToken cancelationToken = default) {
        var callstr = $"{_client.BaseAddress}{endPoint}";
        OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));

        var fileStream = await _client.GetStreamAsync(callstr, cancelationToken);
        using FileStream outputFileStream = new FileStream(fileToSave, FileMode.CreateNew);
        await fileStream.CopyToAsync(outputFileStream);
    }

    public async Task DownloadAsync<T>(string endPoint, T obj, string fileToSave) {
        var ddd = $"{_client.BaseAddress}{endPoint}";

        OnRequisition(this, new RequisitionEventArgs(endPoint, "POST", ""));
        using var response = await _client.PostAsync($"{_client.BaseAddress}{endPoint}", ObjectToHttpContent(obj, "POST", endPoint));
        var result = await response.Content.ReadAsStringAsync();
        using var fileStream = await response.Content.ReadAsStreamAsync();

        using FileStream outputFileStream = new FileStream(fileToSave, FileMode.CreateNew);
        await fileStream.CopyToAsync(outputFileStream);
    }


    public HttpContent ObjectToHttpContent(object obj, string verb, string endpoint) {
        if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string) || obj.GetType() == typeof(decimal)) {
            return new StringContent(obj.ToString());
        }

        HttpContent httpContent = null;
         
        switch (_mediaType) {
            case "application/json" or "multipart/form-data":
                httpContent = SerializeJson(obj, _mediaType, verb, endpoint);
                break;
            case "application/x-www-form-urlencoded":
                httpContent = FormUrlCOntent(obj, verb, endpoint);
                break;
            case "application/xml":
                httpContent = SerializeXml(obj, verb, endpoint);
                break;
        }

        return httpContent;
    }

    private HttpContent FormUrlCOntent(object obj, string verb, string endpoint) {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        OnRequisition(this, new RequisitionEventArgs(endpoint, verb, json));

        var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        var urlContent = new FormUrlEncodedContent(dic);
        return urlContent;
    }

    private HttpContent SerializeJson(object obj, string contentType, string verb, string endpoint) {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        
        OnRequisition(this, new RequisitionEventArgs(endpoint, verb, json));

        return new StringContent(json, Encoding.UTF8, contentType);
    }

    private HttpContent SerializeXml(object obj, string verb, string endpoint) {
        var serializer = new XmlSerializer(obj.GetType());
        var xml = "";

        using (var sww = new StringWriter()) {
            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww)) {
                serializer.Serialize(writer, obj);
                xml = sww.ToString();
            }
        }


        OnRequisition(this, new RequisitionEventArgs(endpoint, verb, xml));
        return new StringContent(xml, Encoding.UTF8, "application/xml");
    }

    public async Task<(T result, string statusCode, string message, HttpResponseHeaders cabecario)> DeserializeResponseAsync<T>(HttpResponseMessage response) {
        var responseContent = await response.Content.ReadAsStringAsync();
        var cabecario = response.Headers;
        var statusCode = response.StatusCode.ToString();
        var result = (default(T), statusCode, responseContent, cabecario);
        try {
            if (response.IsSuccessStatusCode) {
                if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal)) {
                    result = ((T)Convert.ChangeType(responseContent, typeof(T)), statusCode, responseContent, cabecario); ;
                } else if (_mediaType is "application/json" or "multipart/form-data" or "application/x-www-form-urlencoded") {
                    result = (JsonConvert.DeserializeObject<T>(responseContent), statusCode, responseContent, cabecario);
                } else if (_mediaType == "application/xml") {
                    var serializer = new XmlSerializer(typeof(T));
                    using (TextReader reader = new StringReader(responseContent)) {
                        result = ((T)serializer.Deserialize(reader), statusCode, responseContent, cabecario);
                    }
                }

            } else {
                return result;
            }
        } catch (Exception e) {
            result = (default(T), statusCode, responseContent, cabecario);
        }


        OnResponse(this, new ResponseEventArgs("", "", statusCode, result.Item3));

        return result;
    }

    public async Task<(Stream result, string statusCode, HttpResponseHeaders cabecario)> StreamResponseAsync(HttpResponseMessage response) {
        var responseContent = await response.Content.ReadAsStreamAsync();
        var cabecario = response.Headers;
        var statusCode = response.StatusCode.ToString();
        var result = (responseContent, statusCode, cabecario);

        return result;
    }


    private void OnRequisition(object sender, RequisitionEventArgs e) {
        Requisition?.Invoke(sender, e);
    }

    private void OnResponse(object sender, ResponseEventArgs e) {
        Response?.Invoke(sender, e);
    }

    public void Dispose() {
        _client?.Dispose();
    }
}
