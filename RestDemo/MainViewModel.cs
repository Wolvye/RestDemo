using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestDemo
{


    public class MainViewModel
    {
        HttpClient client;
        JsonSerializerOptions _serializerOptions;
        string baseUrl = "https://686e8ed191e85fac429e3a76.mockapi.io";
        private List<User> Users;
        public MainViewModel()
        {
            client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
        }
        public ICommand GetAllUserCommand =>
            new Command(async () =>
            {

                var url = $"{baseUrl}/users";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    //var content = response.Content.ReadAsStringAsync();
                    using (var responseStream =
                        await response.Content.ReadAsStreamAsync())
                    {
                        var data =
                            await JsonSerializer
                            .DeserializeAsync<List<User>>(responseStream, _serializerOptions);
                        Users = data;
                    }
                }
            });


        public ICommand GetSingleUserCommand =>
    new Command(async () =>
    {
        try
        {
            var userId = "2";
            var url = $"{baseUrl}/users/{userId}";
            var json = await client.GetStringAsync(url);

            Console.WriteLine("Antwort vom Server: " + json);

            var user = JsonSerializer.Deserialize<User>(json, _serializerOptions);

            Console.WriteLine($"Name: {user?.name}, Avatar: {user?.avatar}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler: " + ex.Message);
        }
    });

        public ICommand UpdateUserCommand =>
            new Command(async () =>
            {
                var user = Users.FirstOrDefault(x => x.id == "1");
                var url = $"{baseUrl}/1";
                user.name = "John";

                string json =
                    JsonSerializer.Serialize<User>(user, _serializerOptions);

                StringContent content = 
                    new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await client.PutAsync(url, content);
            });

    }
}
