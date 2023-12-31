﻿namespace cloudinteractive_statuspage.Models
{
    //프론트엔드 테스트를 위한 드라이버
    public static class TestModelDriver
    {
        public static DashboardModel CreateModel(List<Models.NotifyItem> notices)
        {
            DashboardModel model = new DashboardModel();
            // Notify
            model.NotifyList = notices;
            model.NotifyList.Add(new NotifyItem(NotifyItem.NotifyType.Info, "현재 시연용 모델을 사용하고 있습니다."));

            // Core services
            model.CoreServiceList.Add(new CoreServiceStateItem("로그온 및 인증 서비스", true));
            model.CoreServiceList.Add(new CoreServiceStateItem("프록시 서비스", true));
            model.CoreServiceList.Add(new CoreServiceStateItem("원격 데스크톱 서비스", false));
            model.CoreServiceList.Add(new CoreServiceStateItem("내부 DNS 서비스", true));
            model.CoreServiceList.Add(new CoreServiceStateItem("iSCSI Target 및 PXE 서비스", false));

            // Services
            model.ServiceList.Add(new ServiceStateItem("C1", "한국 남부", true, false, 90.87f));
            model.ServiceList.Add(new ServiceStateItem("C2", "한국 남부", true, false, 45.12f));
            model.ServiceList.Add(new ServiceStateItem("C3", "한국 중부", true, true, 75.12f));
            model.ServiceList.Add(new ServiceStateItem("아카이브 서비스", "미국 동부", false, false, 99.12f));
            return model;
        }
    }
}
