"""
Модуль для динамического определения доступных действий
Аналог StatisticsCreatorModule/Logic/AvaibleActionTypes.cs
"""
from typing import List, Dict
from app.models.action import ActionAuthorType, VolleyActionType, ActionTypeConverter


class AvailableActionTypes:
    """
    Управляет доступными типами действий в зависимости от состояния игры
    В разных ситуациях (начало игры, между розыгрышами, во время розыгрыша)
    доступны разные действия
    """
    
    def __init__(self, data: Dict[ActionAuthorType, List[VolleyActionType]] = None):
        self._data: Dict[ActionAuthorType, List[VolleyActionType]] = data if data else {}
    
    def add(self, author_type: ActionAuthorType, action_types: List[VolleyActionType]) -> None:
        """Добавляет доступные действия для данного типа автора"""
        self._data[author_type] = action_types
    
    def set_default(self) -> None:
        """Устанавливает все возможные действия для всех авторов"""
        self._data.clear()
        self._data[ActionAuthorType.PLAYER] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.PLAYER
        )
        self._data[ActionAuthorType.OPPONENT_TEAM] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.OPPONENT_TEAM
        )
        self._data[ActionAuthorType.COACH] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.COACH
        )
        self._data[ActionAuthorType.JUDGE] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.JUDGE
        )
    
    def game_beginning(self) -> None:
        """Начало игры - только установка начальной расстановки"""
        self._data.clear()
        self._data[ActionAuthorType.COACH] = [VolleyActionType.START_ARRANGMENT]
    
    def arrangement_set(self) -> None:
        """После установки расстановки - установка начальных параметров"""
        self._data.clear()
        self._data[ActionAuthorType.COACH] = [VolleyActionType.SET_START_PARAMS]
    
    def between_rallies(self, players_actions: List[VolleyActionType]) -> None:
        """
        Между розыгрышами
        Доступны: действия игроков, замены, тайм-ауты, действия судьи и соперника
        """
        self._data.clear()
        self._data[ActionAuthorType.PLAYER] = players_actions
        self._data[ActionAuthorType.OPPONENT_TEAM] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.OPPONENT_TEAM
        )
        self._data[ActionAuthorType.COACH] = [
            VolleyActionType.CHANGE,
            VolleyActionType.TIME_OUT
        ]
        self._data[ActionAuthorType.JUDGE] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.JUDGE
        )
    
    def in_rally(self, players_actions: List[VolleyActionType]) -> None:
        """
        Во время розыгрыша
        Доступны: действия игроков, действия соперника, действия судьи
        НЕ доступны: замены и тайм-ауты тренера
        """
        self._data.clear()
        self._data[ActionAuthorType.PLAYER] = players_actions
        self._data[ActionAuthorType.OPPONENT_TEAM] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.OPPONENT_TEAM
        )
        self._data[ActionAuthorType.JUDGE] = ActionTypeConverter.get_action_types_by_author(
            ActionAuthorType.JUDGE
        )
    
    def clear(self) -> None:
        """Очищает все доступные действия"""
        self._data.clear()
    
    def __getitem__(self, author_type: ActionAuthorType) -> List[VolleyActionType]:
        """Возвращает доступные действия для данного автора"""
        return self._data.get(author_type, [])
    
    def authors(self) -> List[ActionAuthorType]:
        """Возвращает список всех доступных авторов"""
        return list(self._data.keys())
    
    def has_author(self, author_type: ActionAuthorType) -> bool:
        """Проверяет, доступен ли данный автор"""
        return author_type in self._data
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            str(author.value): [action.value for action in actions]
            for author, actions in self._data.items()
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'AvailableActionTypes':
        """Десериализация из словаря"""
        parsed_data = {}
        for author_val, action_vals in data.items():
            author = ActionAuthorType(int(author_val))
            actions = [VolleyActionType(val) for val in action_vals]
            parsed_data[author] = actions
        return AvailableActionTypes(parsed_data)
