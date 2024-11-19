using Minio;

namespace App.Services
{
    public class MinioService
    {
        private readonly IMinioClient Cliente;
        public MinioService()
        {
            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ROOT_USER");
            var secretKey = Environment.GetEnvironmentVariable("MINIO_ROOT_PASSWORD");
            var region = Environment.GetEnvironmentVariable("MINIO_REGION");
            var useSSL = false;
            Cliente = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithRegion(region)
                .WithSSL(useSSL)
                .Build();
        }
        public IMinioClient GetCliente() => Cliente;
    }
}
