"""
Модуль для текстового представления действий (Action Text Representation)
Аналог ActionsLib/TextRepresentation/ActionTextRepresentation.cs
"""
from typing import List, Optional, Dict, Any
from app.models.action import (
    Action, PlayerAction, OpponentAction, JudgeAction, CoachAction,
    VolleyActionType, ActionAuthorType
)
from app.models.player import Player
from app.models.metric import Metric, MetricType, MetricTypeList


class ActionTextRepresentation:
    """Базовый класс для текстового представления действия"""
    
    def __init__(self, author_type: ActionAuthorType):
        self.author_type = author_type
    
    def generate_action(self) -> Optional[Action]:
        """Генерирует объект Action из текстового представления"""
        return None


class PlayerActionTextRepresentation(ActionTextRepresentation):
    """Текстовое представление действия игрока с метриками"""
    
    def __init__(self, action_type: VolleyActionType, metric_type_list: MetricTypeList):
        super().__init__(ActionAuthorType.PLAYER)
        self._action_type = action_type
        self._metric_type_list = metric_type_list
        self._metrics: List[Optional[Metric]] = [None] * len(metric_type_list.metric_types)
        self._short_strings: List[Optional[str]] = [None] * len(metric_type_list.metric_types)
        self._player: Optional[Player] = None
    
    def set_metric_by_object(self, m_type: MetricType, value: Any) -> None:
        """Устанавливает метрику по объекту значения"""
        index = self._metric_type_list.index_of(m_type)
        if index == -1:
            raise ValueError(f"No {m_type.name} metric in action {self._action_type}")
        
        self._metrics[index] = Metric(metric_type=m_type, value=value)
        self._short_strings[index] = self._metrics[index].get_short_string()
    
    def set_metric_by_short_string(self, m_type: MetricType, short_string: str) -> None:
        """Устанавливает метрику по короткой строке"""
        obj = m_type.get_object_by_short_string(short_string)
        self.set_metric_by_object(m_type, obj)
    
    def set_player(self, player: Player) -> None:
        """Устанавливает игрока"""
        self._player = player
    
    def get_player(self) -> Optional[Player]:
        """Возвращает игрока"""
        return self._player
    
    def get_metric(self, metric_type: MetricType) -> Optional[Metric]:
        """Получает метрику по типу"""
        for metric in self._metrics:
            if metric is not None and metric.metric_type == metric_type:
                return metric
        return None
    
    def long_string_format(self) -> str:
        """Форматирует в длинную строку вида: 7:Serve:+:=:6"""
        result = f"{self._player.number}:{self._action_type.name}"
        for short_str in self._short_strings:
            if short_str:
                result += ":" + short_str
        return result
    
    @property
    def action_type(self) -> VolleyActionType:
        return self._action_type
    
    @property
    def metrics(self) -> List[Optional[Metric]]:
        return self._metrics
    
    def contains_metric_type(self, name: str) -> bool:
        """Проверяет наличие метрики с заданным именем"""
        for mt in self._metric_type_list.metric_types:
            if mt.name == name:
                return True
        return False
    
    def get_metric_type(self, name: str) -> Optional[MetricType]:
        """Получает тип метрики по имени"""
        for mt in self._metric_type_list.metric_types:
            if mt.name == name:
                return mt
        return None
    
    def generate_action(self) -> PlayerAction:
        """Генерирует PlayerAction из представления"""
        # Фильтруем только заполненные метрики
        filled_metrics = [m for m in self._metrics if m is not None]
        
        return PlayerAction(
            player=self._player,
            action_type=self._action_type,
            time_code=0.0,  # Будет установлен позже
            points=[],  # Будет установлен позже
            metrics=filled_metrics
        )
    
    def automatic_in_action_filler(self, 
                                   automatic_fillers_rules_holder, 
                                   added_metric: MetricType) -> bool:
        """Автоматическое заполнение метрик на основе правил"""
        result = False
        fillers = automatic_fillers_rules_holder.get_in_action_filler(
            self.action_type, 
            added_metric
        )
        for filler in fillers:
            result = result or filler.use(self)
        return result


class OpponentActionTextRepresentation(ActionTextRepresentation):
    """Текстовое представление действия соперника"""
    
    def __init__(self, action_type: VolleyActionType):
        super().__init__(ActionAuthorType.OPPONENT_TEAM)
        self._action_type = action_type
    
    def generate_action(self) -> OpponentAction:
        """Генерирует OpponentAction"""
        return OpponentAction(action_type=self._action_type)


class JudgeActionTextRepresentation(ActionTextRepresentation):
    """Текстовое представление действия судьи"""
    
    def __init__(self, action_type: VolleyActionType):
        super().__init__(ActionAuthorType.JUDGE)
        self._action_type = action_type
    
    def generate_action(self) -> JudgeAction:
        """Генерирует JudgeAction"""
        return JudgeAction(action_type=self._action_type)


class CoachActionTextRepresentation(ActionTextRepresentation):
    """Текстовое представление действия тренера"""
    
    def __init__(self, action_type: VolleyActionType, players: Optional[List[Player]] = None):
        super().__init__(ActionAuthorType.COACH)
        self._action_type = action_type
        self.players = players if players is not None else []
    
    def generate_action(self) -> CoachAction:
        """Генерирует CoachAction"""
        return CoachAction(
            action_type=self._action_type,
            players=self.players
        )
