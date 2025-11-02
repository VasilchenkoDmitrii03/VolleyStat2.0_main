"""
Главный файл FastAPI приложения для волейбольной статистики
"""
import os
from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
from fastapi.responses import HTMLResponse
from fastapi.templating import Jinja2Templates
from fastapi.middleware.cors import CORSMiddleware
from app.core.config import settings
from app.api import teams, games, players, statistics


# Создание директорий для хранения данных
os.makedirs(settings.STORAGE_PATH, exist_ok=True)
os.makedirs(settings.GAMES_STORAGE_PATH, exist_ok=True)
os.makedirs(settings.TEAMS_STORAGE_PATH, exist_ok=True)

# Создание приложения
app = FastAPI(
    title=settings.APP_NAME,
    version=settings.APP_VERSION,
    description="API для управления волейбольной статистикой",
    docs_url="/api/docs",
    redoc_url="/api/redoc",
    openapi_url="/api/openapi.json"
)

# CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Подключение статических файлов и шаблонов
app.mount("/static", StaticFiles(directory="static"), name="static")
templates = Jinja2Templates(directory="templates")

# Подключение роутеров API
app.include_router(teams.router, prefix="/api/teams", tags=["Teams"])
app.include_router(players.router, prefix="/api/players", tags=["Players"])
app.include_router(games.router, prefix="/api/games", tags=["Games"])
app.include_router(statistics.router, prefix="/api/statistics", tags=["Statistics"])


@app.get("/", response_class=HTMLResponse)
async def root():
    """Главная страница"""
    return """
    <!DOCTYPE html>
    <html>
    <head>
        <title>Volleyball Statistics</title>
        <meta charset="utf-8">
        <style>
            body {
                font-family: Arial, sans-serif;
                max-width: 1200px;
                margin: 0 auto;
                padding: 20px;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                min-height: 100vh;
            }
            .container {
                background: white;
                border-radius: 10px;
                padding: 40px;
                box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            }
            h1 {
                color: #333;
                border-bottom: 3px solid #667eea;
                padding-bottom: 10px;
            }
            .features {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
                gap: 20px;
                margin: 30px 0;
            }
            .feature {
                padding: 20px;
                border: 2px solid #667eea;
                border-radius: 8px;
                transition: transform 0.3s;
            }
            .feature:hover {
                transform: translateY(-5px);
                box-shadow: 0 5px 15px rgba(102, 126, 234, 0.3);
            }
            .feature h3 {
                color: #667eea;
                margin-top: 0;
            }
            .links {
                margin-top: 30px;
            }
            .btn {
                display: inline-block;
                padding: 12px 24px;
                background: #667eea;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                margin-right: 10px;
                transition: background 0.3s;
            }
            .btn:hover {
                background: #764ba2;
            }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>🏐 Volleyball Statistics API</h1>
            <p>Добро пожаловать в систему управления волейбольной статистикой!</p>
            
            <div class="features">
                <div class="feature">
                    <h3>👥 Управление командами</h3>
                    <p>Создавайте и управляйте составами волейбольных команд</p>
                </div>
                <div class="feature">
                    <h3>🎮 Записи игр</h3>
                    <p>Фиксируйте все действия игроков во время матчей</p>
                </div>
                <div class="feature">
                    <h3>📊 Статистика</h3>
                    <p>Анализируйте подробную статистику по игрокам и командам</p>
                </div>
                <div class="feature">
                    <h3>🎥 Видео интеграция</h3>
                    <p>Привязывайте действия к таймкодам видеозаписей</p>
                </div>
            </div>
            
            <div class="links">
                <a href="/api/docs" class="btn">📖 API Документация</a>
                <a href="/dashboard" class="btn">📊 Управление командами</a>
                <a href="/game-setup" class="btn">🎮 Создать новую игру</a>
                <a href="/metrics-config" class="btn">⚙️ Конфигурация метрик</a>
            </div>
        </div>
    </body>
    </html>
    """


@app.get("/dashboard", response_class=HTMLResponse)
async def dashboard():
    """Dashboard для управления статистикой"""
    with open("templates/dashboard.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/game-setup", response_class=HTMLResponse)
async def game_setup():
    """Страница настройки новой игры"""
    with open("templates/game_setup.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/game-recording", response_class=HTMLResponse)
async def game_recording():
    """Страница ведения статистики игры - версия с полными правилами"""
    with open("templates/game_recording_v3.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/game-recording-v3", response_class=HTMLResponse)
async def game_recording_v3():
    """Страница ведения статистики игры V3 - основная рабочая версия"""
    with open("templates/game_recording_v3.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/game-recording-simple", response_class=HTMLResponse)
async def game_recording_simple():
    """Страница ведения статистики игры - упрощённая версия"""
    with open("templates/game_recording_simple.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/game-recording-full", response_class=HTMLResponse)
async def game_recording_full():
    """Страница ведения статистики игры - полная версия с модулями (старая)"""
    with open("templates/game_recording_v2.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/metrics-config", response_class=HTMLResponse)
async def metrics_config():
    """Страница конфигурации метрик, правил автозаполнения и наборов метрик"""
    with open("templates/metrics_config.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/test-api", response_class=HTMLResponse)
async def test_api():
    """Тестовая страница для отладки API"""
    with open("templates/test_api.html", "r", encoding="utf-8") as f:
        return f.read()


@app.get("/health")
async def health_check():
    """Проверка здоровья приложения"""
    return {"status": "healthy", "version": settings.APP_VERSION}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
