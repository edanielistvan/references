from __future__ import print_function

import os.path

from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from google.auth.exceptions import RefreshError

SCOPES = ['https://www.googleapis.com/auth/spreadsheets']

xp_table = {
    "1": "0",
    "2": "300",
    "3": "900",
    "4": "2700",
    "5": "6500",
    "6": "14000",
    "7": "23000",
    "8": "34000",
    "9": "48000",
    "10": "64000",
    "11": "85000",
    "12": "100000",
    "13": "120000",
    "14": "140000",
    "15": "165000",
    "16": "195000",
    "17": "225000",
    "18": "265000",
    "19": "305000",
    "20": "355000"
}

backgrounds = ["Acolyte", "Charlatan", "Criminal", "Entertainer", "Folk Hero",
               "Guild Artisan", "Hermit", "Noble", "Outlander", "Sage", "Sailor", "Soldier", "Urchin"]

races = {
    "Hill Dwarf": "con +2, wis +1",
    "Mountain Dwarf": "con +2, str +2",
    "High Elf": "dex +2, int +1",
    "Wood Elf": "dex +2, wis +1",
    "Drow": "dex +2, cha +1",
    "Lightfoot Halfling": "dex +2, cha +1",
    "Stout Halfling": "dex +2, con +1",
    "Human": "str +1, dex +1, con +1, int +1, wis +1, cha +1",
    "Variant Human": "choose 2: str +1, dex +1, con +1, int +1, wis +1, cha +1",
    "Black Dragonborn": "str +2, cha +1",
    "Blue Dragonborn": "str +2, cha +1",
    "Brass Dragonborn": "str +2, cha +1",
    "Bronze Dragonborn": "str +2, cha +1",
    "Copper Dragonborn": "str +2, cha +1",
    "Gold Dragonborn": "str +2, cha +1",
    "Green Dragonborn": "str +2, cha +1",
    "Red Dragonborn": "str +2, cha +1",
    "Silver Dragonborn": "str +2, cha +1",
    "White Dragonborn": "str +2, cha +1",
    "Forest Gnome": "int +2, dex +1",
    "Rock Gnome": "int +2, con +1",
    "Half-Elf": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Half-Orc": "str +2, con +1",
    "Tiefling": "int +1, cha +2",
    "Protector Aasimar": "cha +2, wis +1",
    "Scourge Aasimar": "cha +2, con +1",
    "Fallen Aasimar": "cha +2, str +1",
    "Firbolg": "wis +2, str +1",
    "Goliath": "str +2, con +1",
    "Kenku": "dex +2, wis +1",
    "Lizardfolk": "con +2, wis +1",
    "Tabaxi": "dex +2, cha +1",
    "Tortle": "str +2, wis +1",
    "Triton": "str +1, con +1, cha +1",
    "Bugbear": "str +2, dex +1",
    "Goblin": "dex +2, con +1",
    "Hobgoblin": "con +2, int +1",
    "Kobold": "dex +2, str -2",
    "Orc": "str +2, con +1, int -2",
    "Yuan-Ti Pureblood": "cha +2, int +1",
    "Duergar": "con +2, str +1",
    "Eladrin": "dex +2, int +1",
    "Sea Elf": "dex +2, con +1",
    "Shadar-Kai": "dex +2, con +1",
    "Githyanki": "str +2, int +1",
    "Githzerai": "wis +2, int +1",
    "Svirfneblin": "int +2, dex +1",
    "Asmodeus Tiefling": "cha +2, int +1",
    "Baalzebul Tiefling": "cha +2, int +1",
    "Dispater Tiefling": "cha +2, dex +1",
    "Fierna Tiefling": "cha +2, wis +1",
    "Glasya Tiefling": "cha +2, dex +1",
    "Levistus Tiefling": "cha +2, con +1",
    "Mammon Tiefling": "cha +2, int +1",
    "Mephistopheles Tiefling": "cha +2, int +1",
    "Zariel Tiefling": "cha +2, str +1",
    "Aarakocra": "dex +2, wis +1",
    "Air Genasi": "con +2, dex +1",
    "Earth Genasi": "con +2, str +1",
    "Fire Genasi": "con +2, int +1",
    "Water Genasi": "con +2, wis +1",
    "Ghostwise Halfling": "dex +2, wis +1",
    "High Half-Elf (Cantrip)": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "High Half-Elf (Weapon Training)": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Wood Half-Elf (Weapon Training)": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Wood Half-Elf (Fleet of Foot)": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Wood Half-Elf (Mask of the Wild)": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Half-Drow": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Aquatic Half-Elf": "cha +2, choose 2: str +1, dex +1, con +1, int +1, wis +1",
    "Devil's Tongue Tiefling": "cha +2, int +1",
    "Hellfire Tiefling": "cha +2, int +1",
    "Winged Tiefling": "cha +2, int +1",
    "Feral Tiefling": "dex +2, int +1",
    "Feral Devil's Tongue Tiefling": "dex +2, int +1",
    "Feral Hellfire Tiefling": "dex +2, int +1",
    "Feral Winged Tiefling": "dex +2, int +1",
    "Changeling": "cha +2, choose 1: str +1, dex +1, con +1, int +1, wis +1, cha +1",
    "Mark of Warding Dwarf": "con +2, int +1",
    "Mark of Shadow Elf": "dex +2, cha +1",
    "Mark of Scribing Gnome": "int +2, cha +1",
    "Mark of Healing Halfling": "dex +2, wis +1",
    "Mark of Hospitality Halfling": "dex +2, cha +1",
    "Mark of Detection Half-Elf": "wis +2, choose 1: str +1, dex +1, con +1, int +1, cha +1",
    "Mark of Storm Half-Elf": "cha +2, dex +1",
    "Mark of Finding Half-Orc": "wis +2, con +1",
    "Mark of Finding Human": "wis +2, con +1",
    "Mark of Handling Human": "wis +2, choose 1: str +1, dex +1, con +1, int +1, cha +1",
    "Mark of Making Human": "int +2, choose 1: str +1, dex +1, con +1, wis +1, cha +1",
    "Mark of Passage Human": "dex +2, choose 1: str +1, con +1, int +1, wis +1, cha +1",
    "Mark of Sentinel Human": "con +2, wis +1",
    "Kalashtar": "wis +2, cha +1",
    "Orc (Eberron)": "str +2, con +1",
    "Beasthide Shifter": "con +2, str +1",
    "Longtooth Shifter": "str +2, dex +1",
    "Swiftstride Shifter": "dex +2, cha +1",
    "Wildhunt Shifter": "wis +2, dex +1",
    "Warforged": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Centaur": "str +2, wis +1",
    "Loxodon": "con +2, wis +1",
    "Minotaur": "str +2, con +1",
    "Simic Hybrid": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Nimble Climber)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Underwater Adaptation)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Nimble Climber Carapace)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Underwater Adaptation Carapace)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Acid Spit)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Simic Hybrid (Carapace)": "con +2, choose 1: str +1, dex +1, int +1, wis +1, cha +1",
    "Vedalken": "int +2, wis +1",
    "Locathah": "str +2, dex +1",
    "Verdan": "cha +2, con +1",
    "Abyssal Tiefling": "cha + 2, con + 1"
}

classes = {
    "Artificer": ["Alchemist", "Artillerist", "Battle Smith"],
    "Barbarian": ["Ancestral Guardian", "Battlerager", "Berserker", "Storm Herald", "Totem Warrior", "Zealot"],
    "Bard": ["Glamour", "Lore", "Swords", "Valor", "Whispers"],
    "Cleric": ["Arcana", "Death", "Forge", "Grave", "Knowledge", "Life", "Light", "Nature", "Order", "Tempest", "Trickery", "War"],
    "Druid": ["Dreams", "Spores", "Land", "Moon", "Shepherd"],
    "Fighter": ["Arcane Archer", "Battle Master", "Cavalier", "Champion", "Eldritch Knight", "Purple Dragon Knight", "Banneret", "Samurai"],
    "Monk": ["Shadow", "Drunken Master", "Four Elements", "Kensei", "Long Death", "Open Hand", "Sun Soul"],
    "Paladin": ['Conquest', 'Devotion', 'Redemption', 'Ancients', 'Crown', 'Vengeance', 'Oathbreaker'],
    "Ranger": ['Beast Master', 'Gloom Stalker', 'Horizon Walker', 'Hunter', 'Monster Slayer'],
    "Rogue": ['Arcane Trickster', 'Assassin', 'Inquisitive', 'Mastermind', 'Scout', 'Swashbuckler', 'Thief'],
    "Sorcerer": ['Divine Soul', 'Draconic Bloodline', 'Shadow Magic', 'Storm Sorcery', 'Wild Magic'],
    "Warlock": ['Archfey', 'Celestial', 'Fiend', 'Great Old One', 'Hexblade', 'Undying'],
    "Wizard": ['Bladesinger', 'Abjuration', 'Conjuration', 'Divination', 'Enchantment', 'Evocation', 'Illusion', 'Necromancy', 'Transmutation', 'War Magic']
}

packs = {
    "Burglar's Pack": [
        "Backpack", "Bag of 1000 ball bearings", "10 feet of string", "Bell", "5#Candle", "Crowbar",
        "Hammer", "10#Piton", "Hooded lantern", "2#Flask of oil", "5#Ration", "Tinderbox", "Waterskin",
        "50 ft. hempen rope"
    ],
    "Diplomat's Pack": [
        "Chest", "2#Case for maps and scrolls", "Fine clothes", "Bottle of ink", "Ink pen", "Lamp",
        "2#Flask of oil", "5#Sheet of paper", "Vial of perfume", "Sealing wax", "Soap"
    ],
    "Dungeoneer's Pack": [
        "Backpack", "Crowbar", "Hammer", "10#Piton", "10#Torch", "Tinderbox", "10#Ration", "Waterskin",
        "50 ft. hempen rope"
    ],
    "Entertainer's Pack": [
        "Backpack", "Bedroll", "2#Costume", "5#Candle", "5#Ration", "Waterskin", "Disguise kit"
    ],
    "Explorer's Pack": [
        "Backpack", "Bedroll", "Mess kit", "10#Torch", "Tinderbox", "10#Ration", "Waterskin",
        "50 ft. hempen rope"
    ],
    "Monster Hunter's Pack": [
        "Chest", "Crowbar", "Hammer", "3#Wooden stake", "Holy symbol", "Flask of holy water", "Manacles",
        "Steel mirror", "Flask of oil", "Tinderbox", "3#Torch"
    ],
    "Priest's Pack": [
        "Backpack", "Blanket", "10#Candle", "Tinderbox", "Alms box", "2#Block of incense", "Censer", "Vestments",
        "2#Ration", "Waterskin"
    ],
    "Scolar's Pack": [
        "Backpack", "Book of lore", "Bottle of ink", "Ink pen", "10#Sheet of parchment", "Little bag of sand", "Small knife"
    ]
}

proficiencies = [
    "Acrobatics", "Animal Handling", "Arcana", "Athletics", "Deception", "History", "Insight", "Intimidation",
    "Investigation", "Medicine", "Nature", "Perception", "Performance", "Persuasion", "Religion",
    "Sleight of Hand", "Stealth", "Survival"
]

sheet_loc = {
    "stats": {
        "str": "C16",
        "dex": "C21",
        "con": "C26",
        "int": "C31",
        "wis": "C36",
        "cha": "C41"
    },
    "race": "T7",
    "classLevels": "T5",
    "playerName": "AE5",
    "characterName": "C6",
    "exp": "AE7",
    "level": "AL6",
    "raceListStart": ["'Race Info'!B8", "'Race Info'!F8"],
    "customRaceStart": [
        "'Race Info'!B118", "'Race Info'!F118", "'Race Info'!I118", "'Race Info'!K118", "'Race Info'!N118",
        "'Race Info'!P118", "'Race Info'!X118", "'Race Info'!AA118", "'Race Info'!AE118", "'Race Info'!AJ118"
    ],
    "customClassStart": [
        "'Class Info'!B106", "'Class Info'!E106", "'Class Info'!I106", "'Class Info'!K106", "'Class Info'!M106",
        "'Class Info'!O106", "'Class Info'!Q106", "'Class Info'!X106", "'Class Info'!AB106", "'Class Info'!AG106",
        "'Class Info'!AJ106", "'Class Info'!AM106"
    ],
    "background": "AJ11",
    "alignment": "AJ28",
    "customBackgroundStart": [
        "AX82", "AY82", "AZ82", "BA82", "BB82"
    ],
    "personalityTrait": "AE12",
    "ideal": "AE16",
    "bond": "AE20",
    "flaw": "AE24",
    "featuresStart": ["C59", "P59", "AC59"],
    "coins": ["'Inventory'!D3", "'Inventory'!D6", "'Inventory'!D9", "'Inventory'!D12", "'Inventory'!D15"],
    "inventoryStart": ["'Inventory'!I3", "'Inventory'!J3", "'Inventory'!R3", "'Inventory'!U3"],
    "packStart": ["'Inventory'!Z3", "'Inventory'!AA3", "'Inventory'!AI3", "'Inventory'!AL3"],
    "profStart": "H25",
    "attackStart": "R32",
    "equipStart": "AC45",
    "spellcastingClass": "C91",
    "age": "C148",
    "height": "F148",
    "weight": "I148",
    "size": "L148",
    "gender": "C150",
    "eyes": "F150",
    "hair": "I150",
    "skin": "L150",
    "appearance": "C176"
}


def ask(question: str) -> str:
    return input(f"{question}: ")


def askYesNoNone(question: str) -> bool:
    result = ask(f"{question} (y/n)")
    return True if result == "y" else False if result == "n" else None


def askYesNo(question: str) -> bool:
    result = askYesNoNone(question)

    while result is None:
        print("Invalid answer! Please use 'y' or 'n'")
        result = askYesNoNone(question)

    return result


def write_values(creds, sheet_id: str, ranges: list, in_opts: str, values: list):
    try:
        service = build('sheets', 'v4', credentials=creds)

        if len(ranges) != len(values):
            print("Error: length of ranges != length of values")
            return

        data = [
            {
                'range': ranges[i],
                'values': [[values[i]]]
            } for i in range(len(values))
        ]

        body = {
            'valueInputOption': in_opts,
            'data': data
        }

        result = service.spreadsheets().values().batchUpdate(
            spreadsheetId=sheet_id,
            body=body
        ).execute()

        print(f"---\n{result['spreadsheetId']} updated!\nTotal rows: {result['totalUpdatedRows']}\nTotal columns: {result['totalUpdatedColumns']}\nTotal cells: {result['totalUpdatedCells']}\nTotal sheets: {result['totalUpdatedSheets']}\n---")
    except HttpError as error:
        print(f"Error: {error}")


def getCreds():
    flow = InstalledAppFlow.from_client_secrets_file(
        'credentials.json', SCOPES)
    return flow.run_local_server(port=0)


def initCreds():
    creds = None
    if os.path.exists('token.json'):
        creds = Credentials.from_authorized_user_file('token.json', SCOPES)

    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            try:
                creds.refresh(Request())
            except RefreshError:
                creds = getCreds()
        else:
            creds = getCreds()

        with open('token.json', 'w') as token:
            token.write(creds.to_json())

    return creds


def first(tup: tuple):
    return tup[0]


def second(tup: tuple):
    return tup[1]


def askGeneric(loc: str, question: str) -> tuple:
    return (loc, ask(question))


def askStat(stat: str) -> tuple:
    return askGeneric(sheet_loc["stats"][stat], stat.upper())


def askPlayerName() -> tuple:
    return askGeneric(sheet_loc["playerName"], "Player Name")


def askCharacterName() -> tuple:
    return askGeneric(sheet_loc["characterName"], "Character Name")


def addLines(loc: str, ind: int) -> str:
    col = ""
    row_s = ""

    for c in loc:
        if c in ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]:
            row_s += c
        else:
            col += c

    row_i = int(row_s) + ind - 1

    return f"{col}{row_i}"


def askRace() -> list:
    race = askGeneric(sheet_loc["race"], "Race")
    check_r = second(race)

    list_r = [race]

    if check_r in races.keys():
        racials = askYesNo(f"Using racials ACIs ({races[check_r]})")

        if not racials:
            ind = list(races).index(check_r)
            s_loc = sheet_loc["raceListStart"][1]

            list_r.append((addLines(s_loc, ind), ask(
                "ASIs (e.g. dex +2, con +1)")))
    else:
        questions = ["Race Name", "ASIs (e.g. dex +2, con +1)", "Natural AC", "Speed", "Bonus HP", "Res/Imm/Vul",
                     "Languages", "Natural Attack (e.g. Bite 1d6 piercing)", "Bonus Weapon/Armor profs.", "Other"]

        list_r.extend(
            askGeneric(sheet_loc["customRaceStart"][i], questions[i]) for i in range(len(questions))
        )

    return list_r


def unknownSubclass(clss: str, sub: str, level: str, offset: int) -> list:
    questions = ["-", "-", "Alt AC", "-", "Init bonus",
                 "Bonus HP/level", "-", "1st lvl profs. (e.g. Light Armor)", "Multiclass profs. (e.g. Light Armor)",
                 "Spellcasting Type", "Spellcasting Ability", "Speed Increase"]
    sp = 0
    list_c = []

    for i in range(len(questions)):
        if questions[i] == "-":
            if sp == 0:
                list_c.append(
                    (addLines(sheet_loc["customClassStart"][0], offset), clss))
            elif sp == 1:
                list_c.append(
                    (addLines(sheet_loc["customClassStart"][1], offset), sub))
            elif sp == 2:
                list_c.append(
                    (addLines(sheet_loc["customClassStart"][3], offset), "-"))
            elif sp == 3:
                list_c.append(
                    (addLines(sheet_loc["customClassStart"][6], offset), "-"))
            sp += 1
        else:
            list_c.append(askGeneric(
                addLines(sheet_loc["customClassStart"][i], offset), questions[i]))

    return list_c


def knownClass(clss: str, level: str) -> list:
    check_s = ask("Subclass").lower().capitalize()
    list_c = []

    if check_s != "":
        list_c.append((sheet_loc["classLevels"], f"{check_s} {clss} {level}"))
        if check_s in classes[clss]:
            list_c += unknownSubclass(clss, check_s, level, 1)
    else:
        list_c.append((sheet_loc["classLevels"],
                      f"{clss} {level}"))

    return list_c


def unknownClass(clss: str, level: str) -> list:
    questions = ["-", "-", "Alt AC", "Hit die", "Init bonus",
                 "Bonus HP/level", "-", "1st lvl profs. (e.g. Light Armor)", "Multiclass profs. (e.g. Light Armor)",
                 "Spellcasting Type", "Spellcasting Ability", "Speed Increase"]
    list_c = []
    sp = 0

    for i in range(len(questions)):
        if questions[i] == "-":
            if sp == 0:
                list_c.append(
                    (sheet_loc["customClassStart"][0], clss))
            elif sp == 1:
                list_c.append(
                    (sheet_loc["customClassStart"][1], "-"))
            elif sp == 2:
                saves = ask("Save profs. (e.g. Intelligence, Wisdom)")
                nskills = ask("Num. of skill choices (e.g. 2)")
                skills = ask(
                    "Class skills (e.g. Arcana, History, Insight, Investigation, Medicine, Religion)")

                prof_string = f"{saves}, Choose {nskills}, {skills}"
                list_c.append(
                    (sheet_loc["customClassStart"][6], prof_string))
            sp += 1
        else:
            list_c.append(askGeneric(
                sheet_loc["customClassStart"][i], questions[i]))

    check_s = ask("Subclass").lower().capitalize()

    if check_s != "":
        list_c.append((sheet_loc["classLevels"], f"{check_s} {clss} {level}"))
        list_c += unknownSubclass(clss, check_s, level, 2)
    else:
        list_c.append((sheet_loc["classLevels"], f"{clss} {level}"))

    return list_c


def askLevelAndClass():
    level = askGeneric(sheet_loc["level"], "Level")
    exp = (sheet_loc["exp"], xp_table[second(level)])

    check_c = ask("Class").lower().capitalize()
    list_c = [level, exp]

    list_c += knownClass(check_c, second(level)
                         ) if check_c in classes else unknownClass(check_c, second(level))

    return list_c


def askMisc() -> list:
    return [askCharacterName(), askPlayerName()]


def askStats() -> list:
    return [askStat(stat) for stat in ["str", "dex", "con", "int", "wis", "cha"]]


def askAlignment() -> list:
    als = ["Lawful Good", "Lawful Neutral", "Lawful Evil", "Neutral Good", "True Neutral", "Neutral Evil",
           "Chaotic Good", "Chaotic Neutral", "Chaotic Evil", "Lawful Jerk", "Chaotic Stupid, Neutral Wuss"]

    result = ask("Alignment")

    while result not in als:
        print(f"Invalid alignment! List: {als}")
        result = ask("Alignment")

    return (sheet_loc["alignment"], result)


def askBackground() -> list:
    check_b = askGeneric(sheet_loc["background"], "Background")

    list_b = [check_b]

    if second(check_b) not in backgrounds:
        questions = ["-", "Background skills (e.g. Insight, Religion)",
                     "Num. of languages", "Num. of tool profs.", "Num. of vehicle profs."]
        sp = 0

        for i in range(len(questions)):
            if questions[i] == "-":
                if sp == 0:
                    list_b.append(
                        (sheet_loc["customBackgroundStart"][0], second(check_b)))
                sp += 1
            else:
                list_b.append(askGeneric(
                    sheet_loc["customBackgroundStart"][i], questions[i]))

    list_b.append(askAlignment())

    list_b.append(askGeneric(
        sheet_loc["personalityTrait"], "Personality trait(s)"))
    list_b.append(askGeneric(sheet_loc["ideal"], "Ideal"))
    list_b.append(askGeneric(sheet_loc["bond"], "Bond"))
    list_b.append(askGeneric(sheet_loc["flaw"], "Flaw"))

    return list_b


def askFeatures() -> list:
    list_f = []
    offset = 1
    current = 0

    while True:
        result = askGeneric(
            addLines(sheet_loc["featuresStart"][current], offset), "Feature")

        if second(result) == "":
            break

        list_f.append(result)

        offset += 1
        if offset > 26:
            current += 1
            offset = 1

    return list_f


def askCoins() -> list:
    coins = ask("Coins (cp, sp, ep, gp, pp)").split(", ")
    return [(sheet_loc["coins"][i], coins[i]) for i in range(len(coins))]


def askItem(offset: int, item: str, pack=False) -> list:
    sep = item.split("#")

    base = "packStart" if pack else "inventoryStart"

    if len(sep) == 1:
        return [(addLines(sheet_loc[base][0], offset), "1"), (addLines(sheet_loc[base][1], offset), sep[0])]
    else:
        return [(addLines(sheet_loc[base][0], offset), sep[0]), (addLines(sheet_loc[base][1], offset), sep[1])]


def askPack():
    result = ask("Pack")

    list_p = []

    if result in packs:
        offset = 1

        for item in enumerate(packs[result]):
            sep = askItem(offset, second(item), True)

            list_p += sep

            offset += 1

    return list_p


def askInventory() -> list:
    list_i = askCoins()
    offset = 1

    list_i += askPack()

    while True:
        result = ask("Inventory (2#Torch, or Torch)")

        if result == "" or offset > 74:
            break

        item = askItem(offset, result)

        list_i += item

        offset += 1

    return list_i


def askProficiencies():
    profs = ask("Proficiencies (e.g. History, Arcana)")

    if profs == "":
        return []

    list_p = []
    split_p = profs.split(", ")

    for p in split_p:
        ind = proficiencies.index(p)
        list_p.append((addLines(sheet_loc["profStart"], ind + 1), "1"))

    return list_p


def askExpertise():
    exps = ask("Expertise (e.g. History, Arcana)")

    if exps == "":
        return []

    list_e = []
    split_e = exps.split(", ")

    for e in split_e:
        ind = proficiencies.index(e)
        list_e.append(addLines(sheet_loc["profStart"], ind + 1), "e")

    return list_e


def askDetails():
    return [
        askGeneric(sheet_loc["age"], "Age"),
        askGeneric(sheet_loc["height"], "Height"),
        askGeneric(sheet_loc["weight"], "Weight"),
        askGeneric(sheet_loc["size"], "Size"),
        askGeneric(sheet_loc["gender"], "Gender"),
        askGeneric(sheet_loc["eyes"], "Eyes"),
        askGeneric(sheet_loc["hair"], "Hair"),
        askGeneric(sheet_loc["skin"], "Skin"),
        askGeneric(sheet_loc["appearance"], "Appearance (link)")
    ]


def askSpellcasting():
    cls = ask("Spellcasting class")

    return [] if cls == "" else [(sheet_loc["spellcastingClass"], cls)]


def askEquips():
    offset = 1
    list_e = []

    while True:
        atk = ask("Attack")

        if atk == "" or offset > 5:
            break

        list_e.append((addLines(sheet_loc["attackStart"], offset), atk))
        offset += 1

    offset = 1

    while True:
        equ = ask("Equipped")

        if equ == "" or offset > 12:
            break

        list_e.append((addLines(sheet_loc["equipStart"], offset), equ))
        offset += 1

    return list_e


def main():
    creds = initCreds()

    bigList = []
    bigList += askStats()
    bigList += askMisc()
    bigList += askRace() + askDetails()
    bigList += askBackground()
    bigList += askLevelAndClass() + askSpellcasting()
    bigList += askProficiencies() + askExpertise()
    bigList += askFeatures()
    bigList += askInventory() + askEquips()

    write_values(creds, ask("Sheet ID"), [first(
        x) for x in bigList], "USER_ENTERED", [second(x) for x in bigList])


if __name__ == '__main__':
    main()
