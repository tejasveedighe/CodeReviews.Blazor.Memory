using System.Net.Http.Json;

namespace MemoryGame.Shared.Services;

public static class SecretExtensions
{
    extension(ISecret secret)
    {
        public bool IsEqual(ISecret secret1)
        {
            return secret.Value.Equals(secret1.Value);
        }
    }
}

public class SecretService(HttpClient httpClient)
{
    public Task<List<ISecret>> GetSecrets(int count = 4)
    {
        return GetImageSecretsAsync(count);
    }

    private async Task<List<ISecret>> GetImageSecretsAsync(int count = 4)
    {
        if (count % 2 > 0)
        {
            throw new InvalidSecretCountRequestedException("The count cannot be odd");
        }
        int c = count / 2;

        var images = await httpClient.GetFromJsonAsync<List<string>>($"/getImages?count={c}");
        if (images is null || images.Count == 0)
        {
            return GetTextSecrets(count);
        }

        // duplicating the first half
        for (int i = 0; i < c; i++)
        {
            images.Add(images[i]);
        }

        List<ISecret> secrets = new();
        foreach (var image in images)
        {
            secrets.Add(
                new Secret
                {
                    Type = typeof(File),
                    HightlightType = HightlightType.Simple,
                    Id = Guid.NewGuid(),
                    Value = $"data:image/png;base64,{image}",
                    Visible = false,
                }
            );
        }

        return secrets.Shuffle().ToList();
    }

    private List<ISecret> GetTextSecrets(int count = 4)
    {
        if (count % 2 > 0)
        {
            throw new InvalidSecretCountRequestedException("The count cannot be odd");
        }

        int c = count / 2;
        List<char> characters = [];
        for (int i = 0; i < c; i++)
        {
            characters.Add((char)(61 + i));
        }

        // duplicate the first half
        for (int i = 0; i < c; i++)
        {
            characters.Add(characters[i]);
        }

        List<ISecret> secrets = [];
        foreach (var character in characters)
        {
            secrets.Add(
                new Secret
                {
                    Id = Guid.NewGuid(),
                    Type = typeof(string),
                    Value = character,
                    Visible = false,
                }
            );
        }

        return secrets.Shuffle().ToList();
    }
}
