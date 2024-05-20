namespace desafio.Models
{

        public class Device
        {
            public string? Identifier { get; set; }
            public string Description { get; set; }
            public string Manufacturer { get; set; }
            public string Url { get; set; }
            public List<CommandDescription> Commands { get; set; }
        }

        public class CommandDescription
        {
            public string Operation { get; set; }
            public string Description { get; set; }
            public Command Command { get; set; }
            public string Result { get; set; }
            public string Format { get; set; }
        }

        public class Command
        {
            public string CommandUnit { get; set; }
            public List<Parameter> Parameters { get; set; }
        }

        public class Parameter
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }

