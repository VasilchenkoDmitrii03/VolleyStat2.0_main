"""
Модели последовательностей действий, розыгрышей и сегментов
"""
from enum import IntEnum
from typing import List, Callable, Optional
from pydantic import BaseModel, Field
from .action import Action, PlayerAction, VolleyActionType, ActionAuthorType


class ActionSegmentResult(IntEnum):
    """Результат сегмента действий"""
    UNDEFINED = -1
    NOT_ENDED = 0
    WON = 1
    LOST = 2
    DISPUTABLE = 3


class ActionSequence(BaseModel):
    """Последовательность действий"""
    actions: List[Action] = Field(default_factory=list, description="Список действий")

    def add(self, action: Action) -> None:
        """Добавить действие"""
        self.actions.append(action)

    def filter_by_author(self, author_type: ActionAuthorType) -> 'ActionSequence':
        """Фильтровать по автору"""
        filtered = ActionSequence()
        for action in self.actions:
            if action.author_type == author_type:
                filtered.add(action)
        return filtered

    def __len__(self) -> int:
        return len(self.actions)

    def __iter__(self):
        return iter(self.actions)

    def __getitem__(self, index):
        return self.actions[index]


class VolleyActionSequence(BaseModel):
    """Последовательность волейбольных действий"""
    actions: List[Action] = Field(default_factory=list, description="Список действий")

    def add(self, action: Action) -> None:
        """Добавить действие"""
        self.actions.append(action)

    def extend(self, sequence: 'VolleyActionSequence') -> None:
        """Добавить последовательность"""
        for action in sequence.actions:
            self.add(action)

    def count_actions_by_condition(self, *conditions: Callable[[PlayerAction], bool]) -> List[int]:
        """Подсчитать действия по условиям"""
        results = [0] * len(conditions)
        for action in self.actions:
            if action.author_type == ActionAuthorType.PLAYER:
                player_action = action
                for i, condition in enumerate(conditions):
                    if condition(player_action):
                        results[i] += 1
        return results

    def select_actions_by_condition(self, condition: Callable[[PlayerAction], bool]) -> 'VolleyActionSequence':
        """Выбрать действия по условию"""
        result = VolleyActionSequence()
        for action in self.actions:
            if action.author_type == ActionAuthorType.PLAYER:
                if condition(action):
                    result.add(action)
        return result

    def __len__(self) -> int:
        return len(self.actions)

    def __iter__(self):
        return iter(self.actions)

    def __getitem__(self, index):
        return self.actions[index]

    def __add__(self, other: 'VolleyActionSequence') -> 'VolleyActionSequence':
        result = VolleyActionSequence()
        result.extend(self)
        result.extend(other)
        return result


class VolleyActionSegment(BaseModel):
    """Сегмент волейбольных действий"""
    actions: VolleyActionSequence = Field(default_factory=VolleyActionSequence, description="Действия сегмента")
    segment_result: ActionSegmentResult = Field(default=ActionSegmentResult.UNDEFINED, description="Результат сегмента")

    def add(self, action: Action) -> None:
        """Добавить действие"""
        self.actions.add(action)

    def contains_action_type(self, action_type: VolleyActionType) -> bool:
        """Проверить наличие типа действия"""
        for action in self.actions:
            if action.action_type == action_type:
                return True
        return False

    def get_action_types(self) -> List[VolleyActionType]:
        """Получить типы действий"""
        return [action.action_type for action in self.actions]

    def get_by_action_type(self, action_type: VolleyActionType) -> Optional[PlayerAction]:
        """Получить действие по типу"""
        for action in self.actions:
            if action.action_type == action_type:
                if isinstance(action, PlayerAction):
                    return action
        return None

    def last(self) -> Optional[Action]:
        """Получить последнее действие"""
        if len(self.actions) > 0:
            return self.actions[-1]
        return None

    @property
    def count(self) -> int:
        """Количество действий"""
        return len(self.actions)


class VolleyActionSegmentSequence(BaseModel):
    """Последовательность сегментов действий"""
    segments: List[VolleyActionSegment] = Field(default_factory=list, description="Список сегментов")

    def add(self, segment: VolleyActionSegment) -> None:
        """Добавить сегмент"""
        self.segments.append(segment)

    def extend(self, sequence: 'VolleyActionSegmentSequence') -> None:
        """Добавить последовательность"""
        for segment in sequence.segments:
            self.add(segment)

    def convert_to_action_sequence(self) -> VolleyActionSequence:
        """Конвертировать в последовательность действий"""
        result = VolleyActionSequence()
        for segment in self.segments:
            result.extend(segment.actions)
        return result

    def select_by_condition(self, condition: Callable[[VolleyActionSegment], bool]) -> 'VolleyActionSegmentSequence':
        """Выбрать сегменты по условию"""
        result = VolleyActionSegmentSequence()
        for segment in self.segments:
            if condition(segment):
                result.add(segment)
        return result

    def __len__(self) -> int:
        return len(self.segments)

    def __iter__(self):
        return iter(self.segments)

    def __getitem__(self, index):
        return self.segments[index]

    @property
    def length(self) -> int:
        """Длина последовательности"""
        return len(self.segments)


class RallyResult(IntEnum):
    """Результат розыгрыша"""
    UNDEFINED = -1
    WON = 0
    LOST = 1
    DISPUTABLE = 2


class Rally(BaseModel):
    """Розыгрыш"""
    segments: VolleyActionSegmentSequence = Field(default_factory=VolleyActionSegmentSequence, description="Сегменты розыгрыша")
    rally_result: RallyResult = Field(default=RallyResult.UNDEFINED, description="Результат розыгрыша")

    def add(self, segment: VolleyActionSegment) -> None:
        """Добавить сегмент"""
        self.segments.add(segment)

    def convert_to_segment_sequence(self) -> VolleyActionSegmentSequence:
        """Конвертировать в последовательность сегментов"""
        return self.segments

    def convert_to_action_sequence(self) -> VolleyActionSequence:
        """Конвертировать в последовательность действий"""
        return self.segments.convert_to_action_sequence()

    def update_rally_result(self) -> None:
        """Обновить результат розыгрыша"""
        if len(self.segments) > 0:
            last_segment = self.segments[-1]
            if last_segment.segment_result == ActionSegmentResult.LOST:
                self.rally_result = RallyResult.LOST
            elif last_segment.segment_result == ActionSegmentResult.WON:
                self.rally_result = RallyResult.WON
            elif last_segment.segment_result == ActionSegmentResult.DISPUTABLE:
                self.rally_result = RallyResult.DISPUTABLE
            else:
                self.rally_result = RallyResult.UNDEFINED

    def get_rally_phase(self):
        """Получить фазу розыгрыша"""
        from .game import SegmentPhase
        if len(self.segments) > 0 and len(self.segments[0].actions) > 0:
            first_action = self.segments[0].actions[0]
            if first_action.action_type == VolleyActionType.SERVE:
                return SegmentPhase.BREAK
            if first_action.action_type == VolleyActionType.RECEPTION:
                return SegmentPhase.RECEP_1
        return SegmentPhase.RECEP

    def rally_time_length(self) -> float:
        """Длительность розыгрыша"""
        first_time_code = -1.0
        last_time_code = -1.0
        
        for segment in self.segments:
            for action in segment.actions:
                if action.author_type == ActionAuthorType.PLAYER:
                    if isinstance(action, PlayerAction):
                        if first_time_code == -1:
                            first_time_code = action.time_code
                        last_time_code = action.time_code
        
        if first_time_code == -1 or last_time_code == -1:
            return 0.0
        return last_time_code - first_time_code

    @property
    def length(self) -> int:
        """Количество сегментов"""
        return len(self.segments)


class RallySequence(BaseModel):
    """Последовательность розыгрышей"""
    rallies: List[Rally] = Field(default_factory=list, description="Список розыгрышей")

    def add(self, rally: Rally) -> None:
        """Добавить розыгрыш"""
        self.rallies.append(rally)

    def extend(self, sequence: 'RallySequence') -> None:
        """Добавить последовательность"""
        for rally in sequence.rallies:
            self.add(rally)

    def convert_to_segment_sequence(self) -> VolleyActionSegmentSequence:
        """Конвертировать в последовательность сегментов"""
        result = VolleyActionSegmentSequence()
        for rally in self.rallies:
            result.extend(rally.convert_to_segment_sequence())
        return result

    def convert_to_action_sequence(self) -> VolleyActionSequence:
        """Конвертировать в последовательность действий"""
        result = VolleyActionSequence()
        for rally in self.rallies:
            result.extend(rally.convert_to_action_sequence())
        return result

    def select_by_condition(self, condition: Callable[[Rally], bool]) -> 'RallySequence':
        """Выбрать розыгрыши по условию"""
        result = RallySequence()
        for rally in self.rallies:
            if condition(rally):
                result.add(rally)
        return result

    def __len__(self) -> int:
        return len(self.rallies)

    def __iter__(self):
        return iter(self.rallies)

    def __getitem__(self, index):
        return self.rallies[index]
