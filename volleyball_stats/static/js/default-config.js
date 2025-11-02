/**
 * Default Kvadratiki Configuration
 * Портировано из /AdditionalFiles/ActionMetricTypes/Kvadratiki
 * Это полная рабочая конфигурация которая использовалась в C# приложении
 */

export const KVADRATIKI_CONFIG = {
    name: "Kvadratiki (Default)",
    description: "Полная конфигурация метрик из оригинального C# приложения",
    metricTypes: [
        {
            name: "Quality",
            shortName: "qual",
            description: "Quality of any action",
            values: [
                { value: 1, shortName: "=" },
                { value: 2, shortName: "-" },
                { value: 3, shortName: "/" },
                { value: 4, shortName: "!" },
                { value: 5, shortName: "+" },
                { value: 6, shortName: "#" }
            ]
        },
        {
            name: "FieldPosition",
            shortName: "fpos",
            description: "Position on field",
            values: [
                { value: 1, shortName: "1" },
                { value: 2, shortName: "2" },
                { value: 3, shortName: "3" },
                { value: 4, shortName: "4" },
                { value: 5, shortName: "5" },
                { value: 6, shortName: "6" }
            ]
        },
        {
            name: "ArrangementPosition",
            shortName: "apos",
            description: "Position in arrangement",
            values: [
                { value: 1, shortName: "1" },
                { value: 2, shortName: "2" },
                { value: 3, shortName: "3" },
                { value: 4, shortName: "4" },
                { value: 5, shortName: "5" },
                { value: 6, shortName: "6" }
            ]
        },
        {
            name: "ServeType",
            shortName: "serT",
            description: "Types of serve",
            values: [
                { value: 1, shortName: "gldr", display: "Glider" },
                { value: 2, shortName: "jmp", display: "Jump" },
                { value: 3, shortName: "shrt", display: "Short" },
                { value: 4, shortName: "lwr", display: "Lower" }
            ]
        },
        {
            name: "Direction",
            shortName: "dir",
            description: "Direction",
            values: [
                { value: 1, shortName: "1" },
                { value: 2, shortName: "2" },
                { value: 3, shortName: "3" },
                { value: 4, shortName: "4" },
                { value: 5, shortName: "5" },
                { value: 6, shortName: "6" }
            ]
        },
        {
            name: "ErrorType",
            shortName: "tyEr",
            description: "Types of errors",
            values: [
                { value: 1, shortName: "noer", display: "NoError" },
                { value: 2, shortName: "foot", display: "Foot" },
                { value: 3, shortName: "net", display: "Net" },
                { value: 4, shortName: "out", display: "Out" },
                { value: 5, shortName: "2tch", display: "Double touch" },
                { value: 6, shortName: "lift", display: "Lift" }
            ]
        },
        {
            name: "AttackComplexity",
            shortName: "attC",
            description: "Complexity of attack",
            values: [
                { value: 1, shortName: "0" },
                { value: 2, shortName: "1" },
                { value: 3, shortName: "2" },
                { value: 4, shortName: "3" },
                { value: 5, shortName: "4" },
                { value: 6, shortName: "5" }
            ]
        },
        {
            name: "ReceptionType",
            shortName: "recT",
            description: "Type of reception",
            values: [
                { value: 1, shortName: "lwr", display: "Lower" },
                { value: 2, shortName: "upr", display: "Upper" },
                { value: 3, shortName: "one", display: "OneHanded" },
                { value: 4, shortName: "pnck", display: "Pancake" }
            ]
        },
        {
            name: "ReceptionQuality",
            shortName: "recQ",
            description: "Quality of reception",
            values: [
                { value: 1, shortName: "=" },
                { value: 2, shortName: "-" },
                { value: 3, shortName: "/" },
                { value: 4, shortName: "!" },
                { value: 5, shortName: "+" },
                { value: 6, shortName: "#" }
            ]
        },
        {
            name: "SetQuality",
            shortName: "setQ",
            description: "Quality of set",
            values: [
                { value: 1, shortName: "=" },
                { value: 2, shortName: "-" },
                { value: 3, shortName: "/" },
                { value: 4, shortName: "!" },
                { value: 5, shortName: "+" },
                { value: 6, shortName: "#" }
            ]
        },
        {
            name: "Combination",
            shortName: "cmb",
            description: "Combination",
            values: [
                { value: 1, shortName: "qcsd", display: "QuickSideout" },
                { value: 2, shortName: "slsd", display: "SlowSideout" },
                { value: 3, shortName: "tkf", display: "Takeoff" },
                { value: 4, shortName: "btkf", display: "BackTakeoff" },
                { value: 5, shortName: "zn", display: "Zone" },
                { value: 6, shortName: "pp", display: "Pipe" },
                { value: 7, shortName: "crs", display: "Cross" },
                { value: 8, shortName: "mtr", display: "Meter" },
                { value: 9, shortName: "trns", display: "Transferring" },
                { value: 10, shortName: "hb", display: "HighBall" },
                { value: 11, shortName: "trn", display: "Train" }
            ]
        },
        {
            name: "BlockersCount",
            shortName: "blkC",
            description: "Count of blockers",
            values: [
                { value: 1, shortName: "0" },
                { value: 2, shortName: "0.5" },
                { value: 3, shortName: "1" },
                { value: 4, shortName: "1.5" },
                { value: 5, shortName: "2" },
                { value: 6, shortName: "2.5" },
                { value: 7, shortName: "3" }
            ]
        },
        {
            name: "AttackType",
            shortName: "attT",
            description: "Type of attack",
            values: [
                { value: 1, shortName: "spk", display: "Spike" },
                { value: 2, shortName: "dmp", display: "Dump" },
                { value: 3, shortName: "blck", display: "BlockOut" },
                { value: 4, shortName: "rcp", display: "Recoup" },
                { value: 5, shortName: "shsp", display: "ShortSpike" }
            ]
        }
    ],
    actionMetrics: {
        "Serve": ["Quality", "FieldPosition", "ArrangementPosition", "ServeType", "Direction", "ErrorType"],
        "Reception": ["Quality", "FieldPosition", "ArrangementPosition", "ServeType", "AttackComplexity", "ReceptionType", "ErrorType"],
        "Set": ["Quality", "FieldPosition", "ArrangementPosition", "ReceptionQuality", "Direction", "Combination", "BlockersCount", "ErrorType"],
        "Attack": ["Quality", "FieldPosition", "ArrangementPosition", "SetQuality", "Combination", "BlockersCount", "Direction", "AttackType", "ErrorType"],
        "Block": ["Quality", "FieldPosition", "ArrangementPosition", "Direction", "Combination", "AttackType", "BlockersCount", "ErrorType"],
        "Defence": ["Quality", "FieldPosition", "ArrangementPosition", "Direction", "Combination", "AttackType", "BlockersCount", "ErrorType"],
        "FreeBall": ["Quality", "FieldPosition", "ArrangementPosition", "ErrorType"],
        "Transfer": ["Quality", "FieldPosition", "ArrangementPosition", "ErrorType"]
    },
    automaticFillers: {
        // InAction fillers - автозаполнение внутри одного действия
        inAction: [
            {
                metricName: "Quality",
                targetMetricName: "ErrorType",
                rules: [
                    { qualityValues: [2, 3, 4, 5, 6], fillValue: 1 } // Quality не "=" → NoError
                ]
            }
        ],
        // Sequence fillers - автозаполнение между действиями
        sequence: [
            {
                sourceAction: "Reception",
                sourceMetric: "Quality",
                targetAction: "Set",
                targetMetric: "ReceptionQuality",
                copyValue: true
            },
            {
                sourceAction: "Set",
                sourceMetric: "Quality",
                targetAction: "Attack",
                targetMetric: "SetQuality",
                copyValue: true
            },
            {
                sourceAction: "Set",
                sourceMetric: "FieldPosition",
                targetAction: "Attack",
                targetMetric: "FieldPosition",
                copyValue: true
            },
            {
                sourceAction: "Set",
                sourceMetric: "Combination",
                targetAction: "Attack",
                targetMetric: "Combination",
                copyValue: true
            },
            {
                sourceAction: "Set",
                sourceMetric: "BlockersCount",
                targetAction: "Attack",
                targetMetric: "BlockersCount",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "Direction",
                targetAction: "Defence",
                targetMetric: "Direction",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "Direction",
                targetAction: "Block",
                targetMetric: "Direction",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "Combination",
                targetAction: "Defence",
                targetMetric: "Combination",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "Combination",
                targetAction: "Block",
                targetMetric: "Combination",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "AttackType",
                targetAction: "Defence",
                targetMetric: "AttackType",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "AttackType",
                targetAction: "Block",
                targetMetric: "AttackType",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "BlockersCount",
                targetAction: "Defence",
                targetMetric: "BlockersCount",
                copyValue: true
            },
            {
                sourceAction: "Attack",
                sourceMetric: "BlockersCount",
                targetAction: "Block",
                targetMetric: "BlockersCount",
                copyValue: true
            }
        ]
    }
};

/**
 * Функция для загрузки дефолтной конфигурации в localStorage
 */
export function loadDefaultConfig() {
    const existingConfigs = JSON.parse(localStorage.getItem('metricsConfigs') || '[]');
    
    // Проверяем, есть ли уже конфигурация Kvadratiki
    const hasKvadratiki = existingConfigs.some(c => c.name === "Kvadratiki (Default)");
    
    if (!hasKvadratiki) {
        existingConfigs.push(KVADRATIKI_CONFIG);
        localStorage.setItem('metricsConfigs', JSON.stringify(existingConfigs));
        console.log('✅ Default Kvadratiki configuration loaded');
        return true;
    }
    
    console.log('ℹ️ Kvadratiki configuration already exists');
    return false;
}

/**
 * Функция для получения конфигурации Kvadratiki
 */
export function getKvadratikiConfig() {
    const configs = JSON.parse(localStorage.getItem('metricsConfigs') || '[]');
    let kvadratiki = configs.find(c => c.name === "Kvadratiki (Default)");
    
    if (!kvadratiki) {
        loadDefaultConfig();
        kvadratiki = KVADRATIKI_CONFIG;
    }
    
    return kvadratiki;
}
