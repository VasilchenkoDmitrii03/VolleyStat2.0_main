"""
Модуль системы фильтрации для статистики
Аналог StatisticsCreatorModule/Filters/FiltersHolder.cs
"""
from typing import List, Dict, Any, Optional
from app.models.action import (
    VolleyActionType, ActionAuthorType, PlayerAction
)
from app.models.metric import MetricType, ActionsMetricTypes
from app.models.sequence import VolleyActionSequence
from app.models.team import Team


class FiltersHolder:
    """
    Держатель фильтров для статистики
    Позволяет фильтровать действия по типу действия и значениям метрик
    """
    
    def __init__(self, amt: ActionsMetricTypes, action_types: List[VolleyActionType]):
        self._amt = amt
        # _data[action_type][metric_type] = list of selected values
        self._data: Dict[VolleyActionType, Dict[MetricType, List[Any]]] = {}
        
        for action_type in action_types:
            self._data[action_type] = {}
            self._fill_action_metric_dict(action_type)
    
    def _fill_action_metric_dict(self, act_type: VolleyActionType) -> None:
        """Заполняет словарь метрик для данного типа действия"""
        metric_type_list = self._amt.get_metric_type_list(act_type)
        if metric_type_list:
            for mt in metric_type_list.metric_types:
                self._fill_metric_dict(act_type, mt)
    
    def _fill_metric_dict(self, act_type: VolleyActionType, metric_type: MetricType) -> None:
        """Заполняет список значений для метрики (изначально все значения)"""
        data = []
        for value in metric_type.acceptable_values.keys():
            data.append(value)
        self._data[act_type][metric_type] = data
    
    def update(self, act_type: VolleyActionType, mt: MetricType, values: List[Any]) -> None:
        """Обновляет фильтр для данного действия и метрики"""
        if act_type in self._data:
            self._data[act_type][mt] = values
    
    def update_by_names(self, act_type: VolleyActionType, mt: MetricType, names: List[str]) -> None:
        """Обновляет фильтр по именам значений"""
        values = []
        for name in names:
            obj = mt.get_object_by_large_name(name)
            values.append(obj)
        self.update(act_type, mt, values)
    
    def clear(self) -> None:
        """Очищает все фильтры"""
        self._data.clear()
    
    def process_sequence(self, seq: VolleyActionSequence) -> VolleyActionSequence:
        """
        Фильтрует последовательность действий согласно установленным фильтрам
        Возвращает только действия, которые проходят все условия фильтрации
        """
        result = VolleyActionSequence()
        
        for action in seq.actions:
            # Фильтруем только действия игроков
            if action.author_type == ActionAuthorType.PLAYER:
                player_action = action
                
                # Проверяем, есть ли фильтр для этого типа действия
                if player_action.action_type in self._data:
                    if self._check_condition(player_action):
                        result.add(action)
        
        return result
    
    def _check_condition(self, player_action: PlayerAction) -> bool:
        """
        Проверяет, проходит ли действие все условия фильтрации
        Действие проходит, если все его метрики входят в выбранные значения фильтров
        """
        result = True
        action_type = player_action.action_type
        metric_type_list = self._amt.get_metric_type_list(action_type)
        
        if not metric_type_list:
            return result
        
        for mt in metric_type_list.metric_types:
            # Получаем значение метрики из действия
            metric = player_action.get_metric(mt)
            if metric:
                # Проверяем, входит ли значение в выбранные для фильтра
                selected_values = self._data[action_type].get(mt, [])
                result = result and (metric.value in selected_values)
        
        return result
    
    def get_selected_values(self, act_type: VolleyActionType, mt: MetricType) -> List[Any]:
        """Возвращает выбранные значения для фильтра"""
        if act_type in self._data and mt in self._data[act_type]:
            return self._data[act_type][mt]
        return []
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        result = {}
        for act_type, metrics_dict in self._data.items():
            result[act_type.value] = {}
            for mt, values in metrics_dict.items():
                result[act_type.value][mt.name] = values
        return result


class PlayersFiltersHolder:
    """
    Держатель фильтров по игрокам
    Позволяет фильтровать действия по номерам игроков
    """
    
    def __init__(self, team: Team):
        self._team = team
        self._selected_numbers: List[int] = []
    
    def update(self, selected_numbers: List[int]) -> None:
        """Обновляет список выбранных номеров игроков"""
        self._selected_numbers = selected_numbers
    
    def update_by_strings(self, selected_strings: List[str]) -> None:
        """Обновляет список выбранных игроков по строкам (например, ["1", "7", "14"])"""
        self._selected_numbers = [int(s) for s in selected_strings]
    
    def clear(self) -> None:
        """Очищает фильтр"""
        self._selected_numbers = []
    
    def process_sequence(self, seq: VolleyActionSequence) -> VolleyActionSequence:
        """
        Фильтрует последовательность по игрокам
        Возвращает только действия выбранных игроков
        """
        result = VolleyActionSequence()
        
        for action in seq.actions:
            if action.author_type == ActionAuthorType.PLAYER:
                player_action = action
                if self._check_condition(player_action):
                    result.add(action)
        
        return result
    
    def _check_condition(self, player_action: PlayerAction) -> bool:
        """Проверяет, входит ли номер игрока в выбранные"""
        # Если список пуст, пропускаем всех
        if not self._selected_numbers:
            return True
        
        return player_action.player.number in self._selected_numbers
    
    def get_selected_numbers(self) -> List[int]:
        """Возвращает список выбранных номеров"""
        return self._selected_numbers
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "selected_numbers": self._selected_numbers
        }


class CombinedFiltersHolder:
    """
    Комбинированный holder для применения и фильтров по метрикам, и фильтров по игрокам
    """
    
    def __init__(self, 
                 amt: ActionsMetricTypes,
                 action_types: List[VolleyActionType],
                 team: Team):
        self.filters_holder = FiltersHolder(amt, action_types)
        self.players_filter = PlayersFiltersHolder(team)
    
    def process_sequence(self, seq: VolleyActionSequence) -> VolleyActionSequence:
        """
        Применяет оба фильтра последовательно:
        1. Фильтр по метрикам действий
        2. Фильтр по игрокам
        """
        # Сначала фильтруем по метрикам
        filtered_by_metrics = self.filters_holder.process_sequence(seq)
        
        # Затем фильтруем по игрокам
        filtered_by_players = self.players_filter.process_sequence(filtered_by_metrics)
        
        return filtered_by_players
    
    def clear(self) -> None:
        """Очищает все фильтры"""
        self.filters_holder.clear()
        self.players_filter.clear()
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "filters": self.filters_holder.to_dict(),
            "players_filter": self.players_filter.to_dict()
        }
