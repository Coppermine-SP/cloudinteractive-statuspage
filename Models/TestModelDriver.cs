namespace cloudinteractive_statuspage.Models
{
    //프론트엔드 테스트를 위한 드라이버
    public static class TestModelDriver
    {
        private static DashboardModel _createModel()
        {
            DashboardModel _model = new DashboardModel();
            _model.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Warn, "현재 시연용 모델이 제공되었습니다."));
            _model.CoreServiceList.Add(new CoreServiceStateItem("로그온 및 인증 서비스", true));
            _model.CoreServiceList.Add(new CoreServiceStateItem("프록시 서비스", true));
            _model.CoreServiceList.Add(new CoreServiceStateItem("원격 데스크톱 서비스", false));
            _model.CoreServiceList.Add(new CoreServiceStateItem("내부 DNS 서비스", true));
            _model.CoreServiceList.Add(new CoreServiceStateItem("iSCSI Target 및 PXE 서비스", false));

            return _model;
        }
        public static DashboardModel Model { get {  return _createModel(); } }
    }
}
