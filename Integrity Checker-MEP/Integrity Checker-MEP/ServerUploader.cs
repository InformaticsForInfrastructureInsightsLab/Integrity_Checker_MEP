using System;
using System.Net;

namespace Integrity_Checker_MEP
{
    public class ServerUploader
    {
        private readonly ILogger _logger;

        public ServerUploader(ILogger logger)
        {
            _logger = logger;
        }

        public string SendToServer(string filePath)
        {
            _logger.Log("서버로 전송 시작");
            try
            {
                using (ExtendedWebClient webClient = new ExtendedWebClient())
                {
                    webClient.AllowWriteStreamBuffering = false;
                    byte[] responseBytes = webClient.UploadFile(ProjectSettings.ServerUploadUrl, filePath);
                    string response = webClient.Encoding.GetString(responseBytes);
                    _logger.Log("서버로 전송 완료");
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.Log("서버 전송 실패: " + e.ToString());
                throw;
            }
        }
    }
}
