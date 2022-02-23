
namespace KBEngine
{
    namespace Const {
        public enum MoveConst //移动状态,进队列的
        {
            Idel = 0,//不动
            Walk = 1,//走
            Run = 2,//跑
            Jump = 3,//跳
            Rush,//冲刺
            Skill,//技能
            ServerMove,//服务端发起的移动
            //PreMove,//p3的预移动
        }

        public enum EntityStage //其他状态
        {
            InBattle = 0,//进战状态
        }
    }
}