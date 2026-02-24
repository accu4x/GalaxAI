using System.Collections.Generic;

namespace Game.Api.Models
{
    public enum SkillTier { Basic = 1, Advanced = 2, Expert = 3, Specialist = 4 }

    public class PlayerSkillDto
    {
        public string SkillId { get; set; }
        public SkillTier Tier { get; set; }
        public double TierProgress { get; set; } // 0-100
        public int TotalXP { get; set; }
        public string Specialty { get; set; } // null if none
        public bool Mastery { get; set; }
    }

    public class SkillDefinitionDto
    {
        public string SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> AllowedSpecialties { get; set; }
    }

    public class UseSkillRequest
    {
        public string ActionContext { get; set; }
    }

    public class InstallChipRequest
    {
        public string ChipItemId { get; set; }
    }

    public class ChooseSpecialtyRequest
    {
        public string SpecialtyId { get; set; }
        public bool Reversible { get; set; }
    }
}
