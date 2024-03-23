namespace dotInstrukcije.Models
{

        public class InstructionsDateWithProfessor
        {
            public DateTime time { get; set; }
            public string status { get; set; }
            public string studentId { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string profilePictureUrl { get; set; }
            public int instructionsCount { get; set; } = 0;
            public List<string> subjects { get; set; }
    }


}
