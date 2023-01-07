using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Web.YmlCustomConfiguration;


public class YmlAsEnviromentVariableConfProvider : ConfigurationProvider
{
    readonly IConfiguration _configuration;
    public YmlAsEnviromentVariableConfProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public override void Load()
    {
        var ymlConf = _configuration["CONFIG"];
        if (string.IsNullOrEmpty(ymlConf))
            return;

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(new UnderscoredNamingConvention())  // see height_in_inches in sample yml 
            .Build();
        var linqYml = deserializer.Deserialize<Dictionary<string, object>>(ymlConf);

        foreach (var key in linqYml.Keys)
        {
            var val = linqYml[key].ToString();

            if (val is null)
                continue;

            _configuration[key.ToUpper()] = val;
        }
    }


}


public class YmlAsEnviromentVariableConfSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new YmlAsEnviromentVariableConfProvider(builder.Build());
}

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddValueFromYmlVar(
        this IConfigurationBuilder builder)
    {
        return builder.Add(new YmlAsEnviromentVariableConfSource());
    }
}