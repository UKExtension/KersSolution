using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{

    public class ProgramStoryViewModel
    {
        public MajorProgram MajorProgram { get; set; }
        public List<MajorProgram> MajorPrograms { get; set; }
        public List<StoryViewModel> Stories { get; set; }

    }
}