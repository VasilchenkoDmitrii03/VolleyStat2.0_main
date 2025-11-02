"""
Модуль для автоматического заполнения метрик действий
Аналог ActionsLib/ActionTypes/AutomaticFillersRulesHolder.cs
"""
from typing import List, Optional, Dict
from dataclasses import dataclass
import json

from app.models.action import VolleyActionType
from app.models.metric import MetricType
from app.models.sequence import VolleyActionSegment


@dataclass
class InActionAutomaticFiller:
    """
    Автоматический филлер для заполнения метрик внутри одного действия
    Логика: если LeftMetric имеет одно из leftValues, то автоматически установить RightMetric = rightValue
    """
    action_type: VolleyActionType
    left_metric: MetricType
    right_metric: MetricType
    left_values: List[str]  # acceptable values names для left_metric
    right_value: str  # acceptable value name для right_metric
    
    def use(self, player_action_text_repr) -> bool:
        """
        Применяет правило автозаполнения
        Returns: True если метрика была установлена
        """
        # Проверка типа действия
        if player_action_text_repr.action_type != self.action_type:
            return False
        
        # Получаем метрики
        right_metric = player_action_text_repr.get_metric(self.right_metric)
        left_metric = player_action_text_repr.get_metric(self.left_metric)
        
        # Если правая метрика уже заполнена, не перезаписываем
        if right_metric is not None:
            return False
        
        # Если левая метрика не заполнена, ничего не делаем
        if left_metric is None:
            return False
        
        # Проверяем, содержится ли значение левой метрики в списке триггеров
        left_value_name = self.left_metric.acceptable_values[str(left_metric.value)]
        if left_value_name in self.left_values:
            # Устанавливаем правую метрику
            right_obj = self.right_metric.get_object_by_large_name(self.right_value)
            player_action_text_repr.set_metric_by_object(self.right_metric, right_obj)
            return True
        
        return False
    
    def __str__(self) -> str:
        left_vals_str = ", ".join(self.left_values)
        return f"{self.action_type.name}: {self.left_metric.name} ({left_vals_str}) ==> {self.right_metric.name}({self.right_value})"
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "action_type": self.action_type.value,
            "left_metric": self.left_metric.to_dict(),
            "right_metric": self.right_metric.to_dict(),
            "left_values": self.left_values,
            "right_value": self.right_value
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'InActionAutomaticFiller':
        """Десериализация из словаря"""
        return InActionAutomaticFiller(
            action_type=VolleyActionType(data["action_type"]),
            left_metric=MetricType.from_dict(data["left_metric"]),
            right_metric=MetricType.from_dict(data["right_metric"]),
            left_values=data["left_values"],
            right_value=data["right_value"]
        )


@dataclass
class SequenceAutomaticFiller:
    """
    Автоматический филлер на основе последовательности действий
    Логика: если предыдущее действие (left) имеет определенную метрику,
    то для текущего действия (right) автоматически установить метрику
    """
    left_action_type: VolleyActionType
    right_action_type: VolleyActionType
    left_metric_type: MetricType
    right_metric_type: MetricType
    left_values: Optional[List[str]] = None  # Если None, то копирование значения
    right_value: Optional[str] = None
    is_copying: bool = False  # Если True, копирует значение метрики напрямую
    
    def use(self, current_action_text_repr, segment: VolleyActionSegment) -> bool:
        """
        Применяет правило автозаполнения на основе последовательности
        Returns: True если метрика была установлена
        """
        # Проверяем тип текущего действия
        if current_action_text_repr.action_type != self.right_action_type:
            return False
        
        # Проверяем наличие левого действия в сегменте
        if not segment.contains_action_type(self.left_action_type):
            return False
        
        # Получаем последнее действие нужного типа
        left_action = segment.get_by_action_type(self.left_action_type)
        if left_action is None:
            return False
        
        # Получаем метрику из левого действия
        left_metric = left_action.get_metric(self.left_metric_type)
        if left_metric is None:
            return False
        
        # Режим копирования
        if self.is_copying:
            current_action_text_repr.set_metric_by_object(
                self.right_metric_type,
                left_metric.value
            )
            return True
        
        # Режим условного заполнения
        if self.left_values is not None:
            left_value_name = self.left_metric_type.acceptable_values[str(left_metric.value)]
            if left_value_name in self.left_values:
                right_obj = self.right_metric_type.get_object_by_large_name(self.right_value)
                current_action_text_repr.set_metric_by_object(
                    self.right_metric_type,
                    right_obj
                )
                return True
        
        return False
    
    def __str__(self) -> str:
        if self.is_copying:
            return f"{self.left_action_type.name}: {self.left_metric_type.name} ==> {self.right_action_type.name}: {self.right_metric_type.name}"
        
        left_vals_str = ", ".join(self.left_values) if self.left_values else ""
        return f"{self.left_action_type.name}: {self.left_metric_type.name} ({left_vals_str}) ==> {self.right_action_type.name}: {self.right_metric_type.name}({self.right_value})"
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "left_action_type": self.left_action_type.value,
            "right_action_type": self.right_action_type.value,
            "left_metric_type": self.left_metric_type.to_dict(),
            "right_metric_type": self.right_metric_type.to_dict(),
            "left_values": self.left_values,
            "right_value": self.right_value,
            "is_copying": self.is_copying
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'SequenceAutomaticFiller':
        """Десериализация из словаря"""
        return SequenceAutomaticFiller(
            left_action_type=VolleyActionType(data["left_action_type"]),
            right_action_type=VolleyActionType(data["right_action_type"]),
            left_metric_type=MetricType.from_dict(data["left_metric_type"]),
            right_metric_type=MetricType.from_dict(data["right_metric_type"]),
            left_values=data.get("left_values"),
            right_value=data.get("right_value"),
            is_copying=data.get("is_copying", False)
        )


class AutomaticFillersRulesHolder:
    """Контейнер для всех правил автоматического заполнения"""
    
    def __init__(self):
        self.in_action_fillers: List[InActionAutomaticFiller] = []
        self.sequence_fillers: List[SequenceAutomaticFiller] = []
    
    def add_in_action_filler(self, filler: InActionAutomaticFiller) -> None:
        """Добавляет InAction filler"""
        self.in_action_fillers.append(filler)
    
    def add_sequence_filler(self, filler: SequenceAutomaticFiller) -> None:
        """Добавляет Sequence filler"""
        self.sequence_fillers.append(filler)
    
    def get_in_action_filler(self, 
                            act_type: VolleyActionType, 
                            metric_type: MetricType) -> List[InActionAutomaticFiller]:
        """Получает все InAction fillers для данного действия и метрики"""
        result = []
        for filler in self.in_action_fillers:
            if filler.action_type == act_type and filler.left_metric == metric_type:
                result.append(filler)
        return result
    
    def get_sequence_fillers(self, right_act: VolleyActionType) -> List[SequenceAutomaticFiller]:
        """Получает все Sequence fillers для данного правого действия"""
        result = []
        for filler in self.sequence_fillers:
            if filler.right_action_type == right_act:
                result.append(filler)
        return result
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "in_action_fillers": [f.to_dict() for f in self.in_action_fillers],
            "sequence_fillers": [f.to_dict() for f in self.sequence_fillers]
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'AutomaticFillersRulesHolder':
        """Десериализация из словаря"""
        holder = AutomaticFillersRulesHolder()
        
        for filler_data in data.get("in_action_fillers", []):
            holder.add_in_action_filler(InActionAutomaticFiller.from_dict(filler_data))
        
        for filler_data in data.get("sequence_fillers", []):
            holder.add_sequence_filler(SequenceAutomaticFiller.from_dict(filler_data))
        
        return holder
    
    def save_to_file(self, filepath: str) -> None:
        """Сохраняет правила в JSON файл"""
        with open(filepath, 'w', encoding='utf-8') as f:
            json.dump(self.to_dict(), f, ensure_ascii=False, indent=2)
    
    @staticmethod
    def load_from_file(filepath: str) -> 'AutomaticFillersRulesHolder':
        """Загружает правила из JSON файла"""
        with open(filepath, 'r', encoding='utf-8') as f:
            data = json.load(f)
        return AutomaticFillersRulesHolder.from_dict(data)
