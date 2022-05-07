
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
            ServerMove = 6,//服务端控制的移动（技能击飞等）
        }

        public enum EntityStage //其他状态
        {
            InBattle = 0,//进战状态
        }
    }
}