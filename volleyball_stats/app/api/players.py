"""
API роутер для работы с игроками
"""
from typing import List
from fastapi import APIRouter, HTTPException, status
from app.models.player import Player, Amplua


router = APIRouter()


@router.get("/ampluas", response_model=dict)
async def get_ampluas():
    """Получить список доступных амплуа"""
    return {
        "ampluas": [
            {"value": Amplua.LIBERO, "name": "Либеро"},
            {"value": Amplua.SETTER, "name": "Связующий"},
            {"value": Amplua.OUTSIDE_HITTER, "name": "Доигровщик"},
            {"value": Amplua.MIDDLE_BLOCKER, "name": "Центральный блокирующий"},
            {"value": Amplua.OPPOSITE, "name": "Диагональный"},
        ]
    }


@router.post("/validate", response_model=dict)
async def validate_player(player: Player):
    """Валидировать данные игрока"""
    return {"valid": True, "player": player}
