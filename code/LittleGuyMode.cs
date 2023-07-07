using Sandbox;
using TerrorTown;
using Player = TerrorTown.Player;

namespace LittleGuyMode
{
    public partial class LittleGuyMode
    {
        [ConVar.Replicated("cutie_little_guy_mode")]
        public static bool LittleGuyModeEnabled { get; set; } = false;
        [ConVar.Replicated("cutie_little_guy_scale")]
        public static float LittleGuyModeScale { get; set; } = 0.5f;
        [ConVar.Replicated("cutie_little_guy_astronaut")]
        public static bool BecomeAnAstronaut { get; set; } = false;
        private static bool PrevLittleGuyModeEnabled { get; set; } = false;
        private static float OrigJumpForce = 0+TerrorTown.WalkController.JumpForce;
        private static float OrigAirAcceleration = 0 + TerrorTown.WalkController.AirAcceleration;

        [GameEvent.Tick.Server]
        public static void OnServerTick()
        {
            if(LittleGuyModeEnabled != PrevLittleGuyModeEnabled)
            {
                foreach(var client in Game.Clients)
                {
                    if(client.Pawn is Player ply)
                    {
                        ScalePlayer(ply);

                        if (!LittleGuyModeEnabled)
                        {
                            TerrorTown.WalkController.JumpForce = OrigJumpForce;
                            TerrorTown.WalkController.AirAcceleration = OrigAirAcceleration;
                        }
                        else if( BecomeAnAstronaut )
                        {
                            TerrorTown.WalkController.JumpForce = OrigJumpForce * (1 / LittleGuyModeScale);
                            TerrorTown.WalkController.AirAcceleration = OrigAirAcceleration * (1 / LittleGuyModeScale);
                        }
                    }
                }

                PrevLittleGuyModeEnabled = LittleGuyModeEnabled;
            }
        }

        public static void ScalePlayer(ModelEntity ply)
        {
            ply.Scale = LittleGuyModeEnabled ? LittleGuyModeScale : 1.0f;
        }

        [Event("Player.PostSpawn")]
        public static void MakePlayersLittleGuys(Player ply)
        {
            if( Game.IsServer ) ScalePlayer(ply);
        }

        [Event("Player.PostOnKilled")]
        public static void AlsoMakeCorpsesLittleGuys(DamageInfo _, Player ply)
        {
            if( Game.IsServer ) ScalePlayer(ply.Corpse);
        }
    }
}
