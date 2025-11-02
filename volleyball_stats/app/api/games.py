"""
API роутер для работы с играми
"""
import json
import os
from typing import List
from fastapi import APIRouter, HTTPException, status
from app.models.game import Game, Set, GameResult
from app.core.config import settings


router = APIRouter()


def get_game_file_path(game_id: str) -> str:
    """Получить путь к файлу игры"""
    return os.path.join(settings.GAMES_STORAGE_PATH, f"{game_id}.json")


def load_games_index() -> dict:
    """Загрузить индекс игр"""
    index_path = os.path.join(settings.GAMES_STORAGE_PATH, "index.json")
    if os.path.exists(index_path):
        with open(index_path, 'r', encoding='utf-8') as f:
            return json.load(f)
    return {}


def save_games_index(index: dict):
    """Сохранить индекс игр"""
    index_path = os.path.join(settings.GAMES_STORAGE_PATH, "index.json")
    with open(index_path, 'w', encoding='utf-8') as f:
        json.dump(index, f, ensure_ascii=False, indent=2)


@router.get("/", response_model=List[dict])
async def get_games():
    """Получить список всех игр"""
    index = load_games_index()
    games = []
    for game_id, game_info in index.items():
        games.append({"id": game_id, **game_info})
    return games


@router.get("/{game_id}", response_model=Game)
async def get_game(game_id: str):
    """Получить игру по ID"""
    file_path = get_game_file_path(game_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Game {game_id} not found")
    
    with open(file_path, 'r', encoding='utf-8') as f:
        game_data = json.load(f)
    
    return Game(**game_data)


@router.post("/", response_model=dict, status_code=status.HTTP_201_CREATED)
async def create_game(game: Game):
    """Создать новую игру"""
    # Генерация ID
    import time
    game_id = f"game_{int(time.time())}"
    
    # Сохранение игры
    file_path = get_game_file_path(game_id)
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(game.model_dump(), f, ensure_ascii=False, indent=2)
    
    # Обновление индекса
    index = load_games_index()
    index[game_id] = {
        "team_name": game.team.name,
        "result": game.result.name,
        "youtube_url": game.youtube_url
    }
    save_games_index(index)
    
    return {"id": game_id, "game": game}


@router.put("/{game_id}", response_model=Game)
async def update_game(game_id: str, game: Game):
    """Обновить игру"""
    file_path = get_game_file_path(game_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Game {game_id} not found")
    
    # Сохранение обновленной игры
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(game.model_dump(), f, ensure_ascii=False, indent=2)
    
    return game


@router.delete("/{game_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_game(game_id: str):
    """Удалить игру"""
    file_path = get_game_file_path(game_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Game {game_id} not found")
    
    os.remove(file_path)
    
    index = load_games_index()
    if game_id in index:
        del index[game_id]
        save_games_index(index)


@router.get("/{game_id}/statistics")
async def get_game_statistics(game_id: str):
    """Получить статистику по игре"""
    game = await get_game(game_id)
    
    # Базовая статистика
    stats = {
        "game_id": game_id,
        "team_name": game.team.name,
        "sets_count": len(game.sets),
        "result": game.result.name,
        "sets_results": [s.set_result.name for s in game.sets]
    }
    
    return stats
