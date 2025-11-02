"""
API роутер для работы с командами
"""
import json
import os
from typing import List
from fastapi import APIRouter, HTTPException, status
from app.models.team import Team
from app.models.player import Player
from app.core.config import settings


router = APIRouter()


def get_team_file_path(team_id: str) -> str:
    """Получить путь к файлу команды"""
    return os.path.join(settings.TEAMS_STORAGE_PATH, f"{team_id}.json")


def load_teams_index() -> dict:
    """Загрузить индекс команд"""
    index_path = os.path.join(settings.TEAMS_STORAGE_PATH, "index.json")
    if os.path.exists(index_path):
        with open(index_path, 'r', encoding='utf-8') as f:
            return json.load(f)
    return {}


def save_teams_index(index: dict):
    """Сохранить индекс команд"""
    index_path = os.path.join(settings.TEAMS_STORAGE_PATH, "index.json")
    with open(index_path, 'w', encoding='utf-8') as f:
        json.dump(index, f, ensure_ascii=False, indent=2)


@router.get("/", response_model=List[Team])
async def get_teams():
    """Получить список всех команд"""
    index = load_teams_index()
    teams = []
    for team_id in index.keys():
        try:
            team = await get_team(team_id)
            teams.append(team)
        except HTTPException:
            continue
    return teams


@router.get("/{team_id}", response_model=Team)
async def get_team(team_id: str):
    """Получить команду по ID"""
    file_path = get_team_file_path(team_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Team {team_id} not found")
    
    with open(file_path, 'r', encoding='utf-8') as f:
        team_data = json.load(f)
    
    team = Team(**team_data)
    team.id = team_id  # Set ID from URL
    return team


@router.post("/", response_model=Team, status_code=status.HTTP_201_CREATED)
async def create_team(team: Team):
    """Создать новую команду"""
    # Генерация ID на основе имени команды
    team_id = team.name.lower().replace(' ', '_').replace('.', '')
    
    # Проверка существования
    if os.path.exists(get_team_file_path(team_id)):
        raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail=f"Team {team.name} already exists")
    
    # Установка ID
    team.id = team_id
    
    # Сохранение команды
    file_path = get_team_file_path(team_id)
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(team.model_dump(), f, ensure_ascii=False, indent=2)
    
    # Обновление индекса
    index = load_teams_index()
    index[team_id] = {"name": team.name, "description": team.description}
    save_teams_index(index)
    
    return team


@router.put("/{team_id}", response_model=Team)
async def update_team(team_id: str, team: Team):
    """Обновить команду"""
    file_path = get_team_file_path(team_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Team {team_id} not found")
    
    # Сохранение обновленной команды
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(team.model_dump(), f, ensure_ascii=False, indent=2)
    
    # Обновление индекса
    index = load_teams_index()
    index[team_id] = {"name": team.name, "description": team.description}
    save_teams_index(index)
    
    return team


@router.delete("/{team_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_team(team_id: str):
    """Удалить команду"""
    file_path = get_team_file_path(team_id)
    if not os.path.exists(file_path):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Team {team_id} not found")
    
    # Удаление файла
    os.remove(file_path)
    
    # Обновление индекса
    index = load_teams_index()
    if team_id in index:
        del index[team_id]
        save_teams_index(index)


@router.post("/{team_id}/players", response_model=Team)
async def add_player_to_team(team_id: str, player: Player):
    """Добавить игрока в команду"""
    team = await get_team(team_id)
    team.add_player(player)
    return await update_team(team_id, team)


@router.delete("/{team_id}/players/{player_number}", response_model=Team)
async def remove_player_from_team(team_id: str, player_number: int):
    """Удалить игрока из команды"""
    team = await get_team(team_id)
    if not team.remove_player(player_number):
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=f"Player #{player_number} not found")
    return await update_team(team_id, team)
