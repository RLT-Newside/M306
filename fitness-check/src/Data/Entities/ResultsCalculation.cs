using System.ComponentModel.DataAnnotations;

namespace FitnessCheckModels
{
    public class ResultsCalculation
    {
        [Key]
        public int ID { get; set; }
        public required string Grade { get; set; }
        public uint Points { get; set; }
        public int MedicineBallPush { get; set; }
        public int StandingLongJump { get; set; }
        public int CoreStrength { get; set; }
        public int OneLegStand { get; set; }
        public int ShuttleRun { get; set; }
        public float TwelveMinutesRun { get; set; }
        public char Gender { get; set; }
    }
}
