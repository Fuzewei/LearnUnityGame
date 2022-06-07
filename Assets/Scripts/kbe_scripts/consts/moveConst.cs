
namespace KBEngine
{
    namespace Const {
        public enum MoveConst //移动状态,进队列的
        {
            Idel = 0,//不动
            Walk = 1,//走
            Run = 2,//跑
            Jump = 3,//跳
            Rush = 4,//冲刺
            Skill = 5,//技能
            ServerMove = 6,//服务端表示产生了突发的移动（技能击飞等,表示服务端触发的突发移动行为）
        }

        public enum AiMoveConst //ai的移动类型
        {
            Idel = 0,//不动
            RANDOM_MOVE = 1,
            ROOTMOTION = 2,
            CHAST_RUN = 3,
            USING_SKILL = 4
        }

        public enum EntityStage //其他状态
        {
            InBattle = 0,//进战状态
        }
    }
}