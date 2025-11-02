"""
Конфигурация приложения
"""
from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    """Настройки приложения"""
    APP_NAME: str = "Volleyball Statistics API"
    APP_VERSION: str = "1.0.0"
    DEBUG: bool = True
    
    # Database
    DATABASE_URL: str = "sqlite+aiosqlite:///./volleyball_stats.db"
    
    # CORS
    CORS_ORIGINS: list = ["*"]
    
    # Storage
    STORAGE_PATH: str = "./storage"
    GAMES_STORAGE_PATH: str = "./storage/games"
    TEAMS_STORAGE_PATH: str = "./storage/teams"
    
    class Config:
        env_file = ".env"


settings = Settings()
