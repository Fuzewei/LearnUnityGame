
namespace KBEngine
{
    namespace Const {
        public enum MoveConst //�ƶ�״̬,�����е�
        {
            Idel = 0,//����
            Walk = 1,//��
            Run = 2,//��
            Jump = 3,//��
            Rush = 4,//���
            Skill = 5,//����
            ServerMove = 6,//����˱�ʾ������ͻ�����ƶ������ܻ��ɵ�,��ʾ����˴�����ͻ���ƶ���Ϊ��
        }

        public enum AiMoveConst //ai���ƶ�����
        {
            Idel = 0,//����
            RANDOM_MOVE = 1,
            ROOTMOTION = 2,
            CHAST_RUN = 3,
            USING_SKILL = 4
        }

        public enum EntityStage //����״̬
        {
            InBattle = 0,//��ս״̬
        }
    }
}