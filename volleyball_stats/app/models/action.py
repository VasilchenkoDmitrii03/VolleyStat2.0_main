"""
Модели действий в волейболе
"""
from enum import IntEnum
from typing import List, Optional, Any
from pydantic import BaseModel, Field
from .player import Player
from .metric import Metric, Point


class ActionAuthorType(IntEnum):
    """Тип автора действия"""
    PLAYER = 0
    OPPONENT_TEAM = 1
    JUDGE = 2
    COACH = 3
    UNDEFINED = -1


class VolleyActionType(IntEnum):
    """Типы волейбольных действий"""
    SERVE = 0
    RECEPTION = 1
    SET = 2
    ATTACK = 3
    BLOCK = 4
    DEFENCE = 5
    TRANSFER = 6
    FREE_BALL = 7
    OPPONENT_ERROR = 8
    OPPONENT_POINT = 9
    JUDGE_MISTAKE_WON = 10
    JUDGE_MISTAKE_LOST = 11
    DISPUTABLE_BALL = 12
    TIME_OUT = 13
    CHANGE = 14
    START_ARRANGEMENT = 15
    SET_START_PARAMS = 16
    UNDEFINED = -1


class Action(BaseModel):
    """Базовый класс действия"""
    name: str = Field(..., description="Название действия")
    author_type: ActionAuthorType = Field(default=ActionAuthorType.UNDEFINED, description="Тип автора действия")
    action_type: VolleyActionType = Field(default=VolleyActionType.UNDEFINED, description="Тип действия")
    metrics: List[Metric] = Field(default_factory=list, description="Метрики действия")

    class Config:
        use_enum_values = False

    def get_metric_types(self) -> List[Any]:
        """Получить типы метрик"""
        types = []
        for metric in self.metrics:
            if metric.metric_type not in types:
                types.append(metric.metric_type)
        return types

    def get_metric_by_type_name(self, type_name: str) -> Optional[Metric]:
        """Получить метрику по имени типа"""
        for metric in self.metrics:
            if metric.metric_type.name == type_name:
                return metric
        return None

    def extended_string(self) -> str:
        """Расширенное строковое представление"""
        result = self.author_type.name
        for metric in self.metrics:
            result += " " + metric.get_short_string()
        return result

    def save_string(self) -> str:
        """Сохранение в строку"""
        result = self.author_type.name
        for metric in self.metrics:
            result += ";" + metric.get_short_string()
        return result


class PlayerAction(Action):
    """Действие игрока"""
    player: Player = Field(..., description="Игрок, совершивший действие")
    time_code: float = Field(default=0.0, ge=0, description="Временная метка в секундах")
    points: List[Point] = Field(default_factory=list, description="Координаты на поле")

    def __init__(self, **data):
        if 'name' not in data and 'action_type' in data:
            data['name'] = VolleyActionType(data['action_type']).name
        if 'author_type' not in data:
            data['author_type'] = ActionAuthorType.PLAYER
        super().__init__(**data)

    class Config:
        json_schema_extra = {
            "example": {
                "name": "ATTACK",
                "author_type": 0,
                "action_type": 3,
                "metrics": [],
                "player": {
                    "name": "Иван",
                    "surname": "Иванов",
                    "height": 195,
                    "number": 10,
                    "amplua": 2
                },
                "time_code": 125.5,
                "points": []
            }
        }

    def get_quality(self) -> int:
        """Получить качество действия"""
        if self.metrics and len(self.metrics) > 0:
            return int(self.metrics[0].value)
        return 0

    def extended_string(self) -> str:
        """Расширенное представление"""
        result = f"#{self.player.number} {self.action_type.name}"
        for metric in self.metrics:
            result += " " + metric.get_short_string()
        result += f" {self.time_code}"
        return result

    def save_string(self) -> str:
        """Сохранение в строку"""
        result = f"#{self.player.number};{self.action_type.name}"
        for metric in self.metrics:
            result += f";{metric.get_short_string()}"
        result += f";{self.time_code}"
        for point in self.points:
            result += ";" + point.save_string()
        return result


class OpponentAction(Action):
    """Действие команды соперника"""
    
    def __init__(self, action_type: VolleyActionType, **data):
        data['name'] = action_type.name
        data['author_type'] = ActionAuthorType.OPPONENT_TEAM
        data['action_type'] = action_type
        super().__init__(**data)

    def extended_string(self) -> str:
        return f"Opponent {self.action_type.name}"

    def save_string(self) -> str:
        return f"Opponent;{self.action_type.name}"


class JudgeAction(Action):
    """Действие судьи"""
    
    def __init__(self, action_type: VolleyActionType, **data):
        data['name'] = action_type.name
        data['author_type'] = ActionAuthorType.JUDGE
        data['action_type'] = action_type
        super().__init__(**data)

    def extended_string(self) -> str:
        return f"Judge {self.action_type.name}"

    def save_string(self) -> str:
        return f"Judge;{self.action_type.name}"


class CoachAction(Action):
    """Действие тренера"""
    players: List[Player] = Field(default_factory=list, description="Игроки, участвующие в действии")

    def __init__(self, action_type: VolleyActionType, players: List[Player] = None, **data):
        data['name'] = action_type.name
        data['author_type'] = ActionAuthorType.COACH
        data['action_type'] = action_type
        data['players'] = players or []
        super().__init__(**data)

    def extended_string(self) -> str:
        result = f"Coach {self.action_type.name}"
        for player in self.players:
            result += f" #{player.number}"
        return result

    def save_string(self) -> str:
        result = f"Coach;{self.action_type.name}"
        for player in self.players:
            result += f";#{player.number}"
        return result


def get_action_types_by_author(author_type: ActionAuthorType) -> List[VolleyActionType]:
    """Получить доступные типы действий для автора"""
    if author_type == ActionAuthorType.JUDGE:
        return [VolleyActionType.JUDGE_MISTAKE_WON, VolleyActionType.JUDGE_MISTAKE_LOST, VolleyActionType.DISPUTABLE_BALL]
    elif author_type == ActionAuthorType.COACH:
        return [VolleyActionType.TIME_OUT, VolleyActionType.CHANGE, VolleyActionType.START_ARRANGEMENT, VolleyActionType.SET_START_PARAMS]
    elif author_type == ActionAuthorType.OPPONENT_TEAM:
        return [VolleyActionType.OPPONENT_ERROR, VolleyActionType.OPPONENT_POINT]
    else:  # PLAYER
        return [VolleyActionType.SERVE, VolleyActionType.RECEPTION, VolleyActionType.SET, 
                VolleyActionType.ATTACK, VolleyActionType.TRANSFER, VolleyActionType.BLOCK, 
                VolleyActionType.DEFENCE, VolleyActionType.FREE_BALL]


def get_action_author(action_type: VolleyActionType) -> ActionAuthorType:
    """Получить автора действия по типу"""
    author_types = [ActionAuthorType.PLAYER, ActionAuthorType.COACH, ActionAuthorType.JUDGE, ActionAuthorType.OPPONENT_TEAM]
    for author_type in author_types:
        if action_type in get_action_types_by_author(author_type):
            return author_type
    return ActionAuthorType.UNDEFINED
