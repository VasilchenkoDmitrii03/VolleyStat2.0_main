"""
Модели метрик
"""
from typing import Any, Dict, List, Optional
from pydantic import BaseModel, Field


class Point(BaseModel):
    """Точка на поле"""
    x: float = Field(..., description="Координата X")
    y: float = Field(..., description="Координата Y")

    def save_string(self) -> str:
        """Сохранить в строку"""
        return f"{self.x}|{self.y}"

    @classmethod
    def load_from_string(cls, s: str) -> 'Point':
        """Загрузить из строки"""
        parts = s.split('|')
        return cls(x=float(parts[0]), y=float(parts[1]))


class MetricType(BaseModel):
    """Тип метрики"""
    name: str = Field(..., description="Название типа метрики")
    description: str = Field(default="unknown", description="Описание типа метрики")
    short_name: str = Field(default="", description="Короткое название")
    value_type: str = Field(default="int", description="Тип значения (int, float, str, point)")
    is_checkable: bool = Field(default=True, description="Проверяемое значение")
    acceptable_values: Dict[str, str] = Field(default_factory=dict, description="Допустимые значения и их названия")
    short_values_names: List[str] = Field(default_factory=list, description="Короткие названия значений")

    class Config:
        json_schema_extra = {
            "example": {
                "name": "Quality",
                "description": "Качество выполнения действия",
                "short_name": "qual",
                "value_type": "int",
                "is_checkable": True,
                "acceptable_values": {
                    "1": "=",
                    "2": "-",
                    "3": "/",
                    "4": "!",
                    "5": "+",
                    "6": "#"
                },
                "short_values_names": ["=", "-", "\\", "!", "+", "#"]
            }
        }

    def get_short_string(self, value: Any) -> str:
        """Получить короткое представление значения"""
        str_value = str(value)
        if str_value in self.acceptable_values:
            index = list(self.acceptable_values.keys()).index(str_value)
            if index < len(self.short_values_names):
                return self.short_values_names[index]
        return str(value)

    def get_object_by_short_string(self, short_string: str) -> Any:
        """Получить значение по короткому представлению"""
        if short_string in self.short_values_names:
            index = self.short_values_names.index(short_string)
            keys = list(self.acceptable_values.keys())
            if index < len(keys):
                value_str = keys[index]
                # Конвертируем в нужный тип
                if self.value_type == "int":
                    return int(value_str)
                elif self.value_type == "float":
                    return float(value_str)
                return value_str
        raise ValueError(f"No '{short_string}' short string in metric type {self.name}")

    def get_object_by_large_name(self, large_name: str) -> Any:
        """Получить значение по полному названию"""
        for key, val in self.acceptable_values.items():
            if val == large_name:
                if self.value_type == "int":
                    return int(key)
                elif self.value_type == "float":
                    return float(key)
                return key
        raise ValueError(f"No '{large_name}' name in metric type {self.name}")

    def is_acceptable_value(self, value: Any) -> bool:
        """Проверить, допустимо ли значение"""
        if not self.is_checkable:
            return True
        return str(value) in self.acceptable_values
    
    @property
    def acceptable_values_names(self) -> List[str]:
        """Возвращает список полных названий значений в порядке ключей"""
        return list(self.acceptable_values.values())
    
    def to_dict(self) -> Dict:
        """Сериализация в словарь"""
        return {
            "name": self.name,
            "description": self.description,
            "short_name": self.short_name,
            "value_type": self.value_type,
            "is_checkable": self.is_checkable,
            "acceptable_values": self.acceptable_values,
            "short_values_names": self.short_values_names
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'MetricType':
        """Десериализация из словаря"""
        return MetricType(**data)


class IntegerMetricType(MetricType):
    """Целочисленный тип метрики"""
    
    def __init__(self, **data):
        data['value_type'] = 'int'
        super().__init__(**data)

    @classmethod
    def create(cls, name: str, description: str, short_name: str,
               acceptable_values: List[int], names: List[str], short_names: List[str]) -> 'IntegerMetricType':
        """Создать целочисленный тип метрики"""
        if len(acceptable_values) != len(names) or len(acceptable_values) != len(short_names):
            raise ValueError("Number of values does not match the number of names")
        
        values_dict = {str(val): name for val, name in zip(acceptable_values, names)}
        
        return cls(
            name=name,
            description=description,
            short_name=short_name,
            acceptable_values=values_dict,
            short_values_names=short_names
        )

    @classmethod
    def create_simple(cls, name: str, description: str, short_name: str,
                     acceptable_values: List[int]) -> 'IntegerMetricType':
        """Создать простой целочисленный тип метрики (названия = значения)"""
        names = [str(val) for val in acceptable_values]
        return cls.create(name, description, short_name, acceptable_values, names, names)


class Metric(BaseModel):
    """Метрика"""
    metric_type: MetricType = Field(..., description="Тип метрики")
    value: Any = Field(..., description="Значение метрики")

    def __init__(self, **data):
        super().__init__(**data)
        # Проверяем допустимость значения
        if not self.metric_type.is_acceptable_value(self.value):
            raise ValueError(f"Non acceptable value {self.value} in metric type {self.metric_type.name}")

    class Config:
        json_schema_extra = {
            "example": {
                "metric_type": {
                    "name": "Quality",
                    "description": "Качество действия",
                    "short_name": "qual",
                    "value_type": "int",
                    "acceptable_values": {
                        "1": "Ошибка",
                        "5": "Отлично"
                    },
                    "short_values_names": ["=", "+"]
                },
                "value": 5
            }
        }

    def get_short_string(self) -> str:
        """Получить короткое представление"""
        return self.metric_type.get_short_string(self.value)


# Создание стандартных типов метрик
def create_default_metrics() -> List[MetricType]:
    """Создать метрики по умолчанию"""
    quality = MetricType(
        name="Quality",
        description="Quality of any action",
        short_name="qual",
        value_type="int",
        acceptable_values={
            "1": "=",
            "2": "-",
            "3": "/",
            "4": "!",
            "5": "+",
            "6": "#"
        },
        short_values_names=["=", "-", "\\", "!", "+", "#"]
    )
    
    field_position = IntegerMetricType.create_simple(
        "FieldPosition",
        "Position on field",
        "fpos",
        [1, 2, 3, 4, 5, 6]
    )
    
    arrangement_position = IntegerMetricType.create_simple(
        "ArrangementPosition",
        "Position in arrangement",
        "apos",
        [1, 2, 3, 4, 5, 6]
    )
    
    return [quality, field_position, arrangement_position]


class MetricTypeList:
    """Список типов метрик для определенного типа действия"""
    
    def __init__(self, metric_types: Optional[List[MetricType]] = None):
        self.metric_types: List[MetricType] = metric_types if metric_types else []
    
    def add(self, metric_type: MetricType) -> None:
        """Добавить тип метрики"""
        self.metric_types.append(metric_type)
    
    def index_of(self, metric_type: MetricType) -> int:
        """Получить индекс типа метрики"""
        try:
            return self.metric_types.index(metric_type)
        except ValueError:
            # Попробуем найти по имени
            for i, mt in enumerate(self.metric_types):
                if mt.name == metric_type.name:
                    return i
            return -1
    
    def __len__(self) -> int:
        """Длина списка"""
        return len(self.metric_types)
    
    def __iter__(self):
        """Итерация по типам метрик"""
        return iter(self.metric_types)
    
    def __getitem__(self, index: int) -> MetricType:
        """Получить тип метрики по индексу"""
        return self.metric_types[index]
    
    def to_dict(self) -> Dict:
        """Сериализация"""
        return {
            "metric_types": [mt.to_dict() for mt in self.metric_types]
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'MetricTypeList':
        """Десериализация"""
        metric_types = [MetricType.from_dict(mt_data) for mt_data in data.get("metric_types", [])]
        return MetricTypeList(metric_types)


class ActionsMetricTypes:
    """
    Контейнер для типов метрик всех действий
    Связывает каждый тип действия с его списком типов метрик
    """
    
    def __init__(self):
        self._data: Dict = {}
        # Здесь можно добавить fillers_rules
        from app.logic.automatic_fillers import AutomaticFillersRulesHolder
        self.fillers_rules = AutomaticFillersRulesHolder()
    
    def add(self, action_type, metric_type_list: MetricTypeList) -> None:
        """Добавить список метрик для типа действия"""
        self._data[action_type] = metric_type_list
    
    def get_metric_type_list(self, action_type) -> Optional[MetricTypeList]:
        """Получить список типов метрик для действия"""
        return self._data.get(action_type)
    
    def __getitem__(self, action_type) -> MetricTypeList:
        """Получить MetricTypeList по типу действия"""
        if action_type not in self._data:
            raise KeyError(f"No metrics for action type {action_type}")
        return self._data[action_type]
    
    def to_dict(self) -> Dict:
        """Сериализация"""
        return {
            "action_metrics": {
                str(act_type.value): mtl.to_dict()
                for act_type, mtl in self._data.items()
            },
            "fillers_rules": self.fillers_rules.to_dict()
        }
    
    @staticmethod
    def from_dict(data: Dict) -> 'ActionsMetricTypes':
        """Десериализация"""
        from app.models.action import VolleyActionType
        from app.logic.automatic_fillers import AutomaticFillersRulesHolder
        
        amt = ActionsMetricTypes()
        
        for act_type_str, mtl_data in data.get("action_metrics", {}).items():
            act_type = VolleyActionType(int(act_type_str))
            mtl = MetricTypeList.from_dict(mtl_data)
            amt.add(act_type, mtl)
        
        if "fillers_rules" in data:
            amt.fillers_rules = AutomaticFillersRulesHolder.from_dict(data["fillers_rules"])
        
        return amt
    
    @staticmethod
    def create_default() -> 'ActionsMetricTypes':
        """Создать стандартный набор метрик для всех действий"""
        from app.models.action import VolleyActionType
        
        amt = ActionsMetricTypes()
        
        # Quality метрика для всех действий
        quality = MetricType(
            name="Quality",
            description="Качество выполнения действия",
            short_name="qual",
            value_type="int",
            acceptable_values={
                "1": "Ошибка (=)",
                "2": "Плохо (-)",
                "3": "Средне (/)",
                "4": "Хорошо (!)",
                "5": "Отлично (+)",
                "6": "Идеально (#)"
            },
            short_values_names=["=", "-", "/", "!", "+", "#"]
        )
        
        # FieldPosition и ArrangementPosition
        field_position = IntegerMetricType.create_simple(
            "FieldPosition",
            "Позиция на поле (зона 1-6)",
            "fpos",
            [1, 2, 3, 4, 5, 6]
        )
        
        arrangement_position = IntegerMetricType.create_simple(
            "ArrangementPosition",
            "Позиция в расстановке (P1-P6)",
            "apos",
            [1, 2, 3, 4, 5, 6]
        )
        
        # Базовые метрики для большинства действий игроков
        basic_metrics = MetricTypeList([quality, field_position, arrangement_position])
        
        # Добавляем для всех основных действий
        for action_type in [
            VolleyActionType.SERVE,
            VolleyActionType.RECEPTION,
            VolleyActionType.SET,
            VolleyActionType.ATTACK,
            VolleyActionType.BLOCK,
            VolleyActionType.DEFENCE,
            VolleyActionType.FREE_BALL,
            VolleyActionType.TRANSFER
        ]:
            amt.add(action_type, basic_metrics)
        
        return amt
