using Fusion;

namespace DonzaiGamecorp.TicTacToe
{
    public class PlayerDataManager : NetworkBehaviour
    {
        public string NickName;
        public string OpponentName;

        public int PlayerAvatarNum;
        public int OpponentAvatarNum;

        public int PlayerThrophyCount;
        public int OpponentThrophyCount;

        public NetworkObject LocalPlayerObj;
        public NetworkObject RemotePlayerObj;

        public PlayerRef LocalPlayerRef;
        public PlayerRef RemotePlayerRef;

        public bool _isOurTurnToPlay = false;
        public bool didHostReqRematch = false;
        public bool didClientReqRematch = false;
    }
}

