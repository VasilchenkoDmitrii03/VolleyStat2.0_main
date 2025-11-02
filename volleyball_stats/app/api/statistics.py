"""
API роутер для работы со статистикой
"""
from fastapi import APIRouter, HTTPException
from app.models.action import VolleyActionType
from .games import get_game


router = APIRouter()


@router.get("/action-types")
async def get_action_types():
    """Получить список типов действий"""
    return {
        "action_types": [
            {"value": VolleyActionType.SERVE, "name": "Подача"},
            {"value": VolleyActionType.RECEPTION, "name": "Прием"},
            {"value": VolleyActionType.SET, "name": "Передача"},
            {"value": VolleyActionType.ATTACK, "name": "Атака"},
            {"value": VolleyActionType.BLOCK, "name": "Блок"},
            {"value": VolleyActionType.DEFENCE, "name": "Защита"},
            {"value": VolleyActionType.TRANSFER, "name": "Переход"},
            {"value": VolleyActionType.FREE_BALL, "name": "Свободный мяч"},
        ]
    }


@router.get("/games/{game_id}/player/{player_number}")
async def get_player_statistics(game_id: str, player_number: int):
    """Получить статистику игрока в игре"""
    game = await get_game(game_id)
    
    player = game.team.get_player_by_number(player_number)
    if not player:
        raise HTTPException(status_code=404, detail="Player not found")
    
    # Получить все действия игрока
    all_actions = game.get_volley_action_sequence()
    player_actions = []
    
    for action in all_actions:
        if hasattr(action, 'player') and action.player.number == player_number:
            player_actions.append(action)
    
    # Подсчет статистики по типам действий
    action_counts = {}
    for action in player_actions:
        action_type_name = action.action_type.name
        if action_type_name not in action_counts:
            action_counts[action_type_name] = 0
        action_counts[action_type_name] += 1
    
    return {
        "player": player,
        "total_actions": len(player_actions),
        "actions_by_type": action_counts
    }


@router.get("/games/{game_id}/team")
async def get_team_statistics(game_id: str):
    """Получить командную статистику"""
    game = await get_game(game_id)
    
    all_actions = game.get_volley_action_sequence()
    
    # Статистика по типам действий
    action_counts = {}
    for action in all_actions:
        action_type_name = action.action_type.name
        if action_type_name not in action_counts:
            action_counts[action_type_name] = 0
        action_counts[action_type_name] += 1
    
    return {
        "team_name": game.team.name,
        "total_actions": len(all_actions),
        "actions_by_type": action_counts,
        "sets_played": len(game.sets),
        "game_result": game.result.name
    }
