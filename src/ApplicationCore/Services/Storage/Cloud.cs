using ApplicationCore.Settings;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ApplicationCore.Services;

public interface ICloudStorageService
{
	Task<string> UploadFileAsync(IFormFile imageFile, string fileName);
	Task<string> UploadFileAsync(string filePath, string fileName);
	Task DeleteFileAsync(string fileName);
}

public class GoogleStorageService : ICloudStorageService
{
	private readonly GoogleCredential _googleCredential;
	private readonly StorageClient _storageClient;
	private readonly string _bucketName;

	public GoogleStorageService(IOptions<CloudStorageSettings> cloudStorageSettings)
	{
		_googleCredential = GoogleCredential.FromFile(cloudStorageSettings.Value.CredentialPath);
		_storageClient = StorageClient.Create(_googleCredential);
		_bucketName = cloudStorageSettings.Value.Bucket;
	}

	public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
	{
		using (var memoryStream = new MemoryStream())
		{
			await imageFile.CopyToAsync(memoryStream);
			var dataObject = await _storageClient.UploadObjectAsync(_bucketName, fileNameForStorage, null, memoryStream);
			return dataObject.MediaLink;
		}
	}

	public async Task<string> UploadFileAsync(string filePath, string name)
	{
		using (var memoryStream = new MemoryStream())
		{
			using (var stream = new FileStream(filePath, FileMode.Open))
			{
				await stream.CopyToAsync(memoryStream);
			}

			var dataObject = await _storageClient.UploadObjectAsync(_bucketName, name, null, memoryStream);
			return dataObject.MediaLink;
		}
	}

	public async Task DeleteFileAsync(string fileNameForStorage)
	{
		await _storageClient.DeleteObjectAsync(_bucketName, fileNameForStorage);
	}
}
