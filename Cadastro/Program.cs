using Cadastro.Models.Services.Application.Settings.Geral;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cadastro {
    public class Program {

        public static Config Config = null;

        public static readonly string PathWebmanifest = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "wwwroot", "webmanifest.json");

        public object JsonDeserialize { get; private set; }

        public static async Task Main(string[] args) {
            await Program.LoadConfigs();
            CreateHostBuilder(args).Build().Run();
        }

        //#########################################################################################################
        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder => {
                  webBuilder.UseStartup<Startup>();
              });

        //#########################################################################################################
        //Limite máximo de requisição aceita pelo Kestrell em bytes
        public static IWebHost BuildWebHost(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options => {
                    options.Limits.MaxRequestBodySize = 50000000;
                })
                .UseIISIntegration()
                .Build();
        }

        //#########################################################################################################
        public static async Task LoadConfigs() {
            Config config;
            string json = null;

            //Cria arquivo de configuração caso não exista
            if(!File.Exists(Config.PathConfig)) {

                Console.WriteLine("Criando arquivo de configuração padrão...");

                config = new Config();
                config.SetAllDefault(true);

                json = config.GetJsonString();

                if(!Directory.Exists(Path.GetDirectoryName(Config.PathConfig)))
                    Directory.CreateDirectory(Path.GetDirectoryName(Config.PathConfig));
                StreamWriter fileW = new StreamWriter(Config.PathConfig, false, Encoding.UTF8);
                await fileW.WriteAsync(json);
                await fileW.DisposeAsync();

                Program.Config = config;
                Console.WriteLine("Arquivo de configuração padrão criado.");
            } else {//-------------------------------------------------------------------------------------------------

                Console.WriteLine("Lendo arquivo de configuração...");
                StreamReader fileR = new StreamReader(Config.PathConfig, Encoding.UTF8);

                json += await fileR.ReadToEndAsync();
                fileR.Dispose();

                try {
                    if(String.IsNullOrWhiteSpace(json))
                        throw new Exception();
                    JsonSerializerOptions opt = new JsonSerializerOptions() {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        IgnoreNullValues = true
                    };
                    config = JsonSerializer.Deserialize<Config>(json, opt);
                    config.SetAllDefault();
                    Console.WriteLine("Leitura do arquivo de configuração feita com sucesso!");
                } catch {
                    Console.WriteLine("Ocorreu um erro ao tentar ler o arquivo de configuração.");
                    Console.WriteLine("Processo de reconfiguração padrão iniciada.");
                    int cErroFile = 1;
                    string newFile;
                    do {
                        newFile = Config.PathConfig.Replace("config.json", $"config_erro({cErroFile++}).json");
                    } while(File.Exists(newFile));
                    File.Copy(Config.PathConfig, newFile);
                    File.Delete(Config.PathConfig);
                    await Program.LoadConfigs();
                    return;
                }

                //Atribui valores nulos e salva nova conf
                try {
                    Console.WriteLine("Verificando pendências no arquivo de configuração.");
                    json = config.GetJsonString();
                    StreamWriter fileW = new StreamWriter(Config.PathConfig, false, Encoding.UTF8);
                    await fileW.WriteAsync(json);
                    await fileW.DisposeAsync();
                    Console.WriteLine("Concluído! Configuração carregada e pronta para uso.");
                } catch(Exception err) {
                    throw err;
                }

                Program.Config = config;
            }
            string segundaLinha = "";

            //Coleta a segunda linha do webmanifest
            using(StreamReader webM = new StreamReader(PathWebmanifest)) {
                for(int w = 0; w <= 1; w++)
                    segundaLinha = await webM.ReadLineAsync();
            }

            if(!Regex.IsMatch(segundaLinha, $"\"{Config.nomeSistema.Trim()}\""))
                await ChangeWebmanifest();
        }

        //#########################################################################################################
        private static async Task ChangeWebmanifest() {
            string modelo = "";

            string PathDefault = $"{Path.GetDirectoryName(PathWebmanifest)}{Path.DirectorySeparatorChar}webmanifest";
            //Coleta o modelo padrão do webmanifest
            StreamReader modeloS = new StreamReader(PathDefault, Encoding.UTF8);
            modelo = await modeloS.ReadToEndAsync();
            modeloS.Dispose();

            modelo = modelo.Replace("#empresa", Config.nomeSistema.Trim());

            //Grava o modelo modificado no arquivo do webmanifest
            StreamWriter modeloW = new StreamWriter(PathWebmanifest, false, Encoding.UTF8);
            await modeloW.WriteAsync(modelo);
            await modeloW.DisposeAsync();
        }

    }
}
