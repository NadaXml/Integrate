using log_service;
namespace game_service {
    public interface IFunProvider {
        NLogService logService { get; set; }
    }
}
