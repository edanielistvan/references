use crate::cube::find_best_moves;

mod cube {
    use std::fmt;
    use std::vec;
    use rand::Rng;

    #[derive(Copy, Clone, PartialEq)]
    pub enum Color {
        White, Yellow, Red, Orange, Blue, Green 
    }

    #[derive(Copy, Clone, PartialEq)]
    pub enum Turn {
        U, D, R, L, F, B, M, E, S, Up, Dp, Rp, Lp, Fp, Bp, Mp, Ep, Sp, None
    }

    #[derive(Copy, Clone)]
    pub struct Cube {
        pub cube: [Color; 54],
        pub solved: bool
    }

    //#[derive(Copy, Clone)]
    pub struct Score {
        pub turns: Vec<Turn>,
        pub score: i32
    }

    impl fmt::Display for Turn {
        fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
            let x = match self {
                Turn::U => "U",
                Turn::D => "D",
                Turn::R => "R",
                Turn::L => "L", 
                Turn::F => "F",
                Turn::B => "B",
                Turn::M => "M",
                Turn::E => "E",
                Turn::S => "S",
                Turn::Up => "U'",
                Turn::Dp => "D'",
                Turn::Rp => "R'",
                Turn::Lp => "L'", 
                Turn::Fp => "F'",
                Turn::Bp => "B'",
                Turn::Mp => "M'",
                Turn::Ep => "E'",
                Turn::Sp => "S'",
                Turn::None => "",
            };

            write!(f,"{}",x)
        }
    }

    impl fmt::Display for Color {
        fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
            let x = match self {
                Color::White => "W",
                Color::Yellow => "Y",
                Color::Red => "R",
                Color::Orange => "O",
                Color::Blue => "B",
                Color::Green => "G",
            };

            write!(f,"{}",x)
        }
    }

    impl fmt::Display for Cube {
        fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
            let c = &self.cube;
            let solved = self.solved;

            let s = match solved {
                true => "SOLVED",
                false => "NOT SOLVED",
            };

            write!(
                f,
                "|{}|\n-----------------------\n      {} {} {}\n      {} {} {}\n      {} {} {}\n{} {} {} {} {} {} {} {} {} {} {} {}\n{} {} {} {} {} {} {} {} {} {} {} {}\n{} {} {} {} {} {} {} {} {} {} {} {}\n      {} {} {}\n      {} {} {}\n      {} {} {}\n-----------------------",
                s,c[0],c[1],c[2],c[3],c[4],c[5],c[6],c[7],c[8],c[36],c[37],c[38],c[9],c[10],c[11],c[18],c[19],c[20],c[27],c[28],c[29],
                c[39],c[40],c[41],c[12],c[13],c[14],c[21],c[22],c[23],c[30],c[31],c[32],c[42],c[43],c[44],c[15],c[16],c[17],c[24],
                c[25],c[26],c[33],c[34],c[35],c[45],c[46],c[47],c[48],c[49],c[50],c[51],c[52],c[53]
            )
        }
    }

    fn solved_cube() -> [Color; 54] {
        [
            Color::White, Color::White, Color::White, Color::White, Color::White, Color::White, Color::White, Color::White, Color::White,
            Color::Blue, Color::Blue, Color::Blue, Color::Blue, Color::Blue, Color::Blue, Color::Blue, Color::Blue, Color::Blue,
            Color::Orange, Color::Orange, Color::Orange, Color::Orange, Color::Orange, Color::Orange, Color::Orange, Color::Orange, Color::Orange,
            Color::Green, Color::Green, Color::Green, Color::Green, Color::Green, Color::Green, Color::Green, Color::Green, Color::Green,
            Color::Red, Color::Red, Color::Red, Color::Red, Color::Red, Color::Red, Color::Red, Color::Red, Color::Red,
            Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow, Color::Yellow
        ]
    }

    fn count_points(cb: Cube) -> i32 {
        let solved = solved_cube();

        let mut points = 0;

        for i in 0..solved.len() {
            if solved[i] == cb.cube[i] {
                points = points + 1;
            } 
        }

        points
    }

    pub fn is_solved(arr: [Color; 54]) -> bool {
        let mut i = 0;
        let mut wrong = false;
        let solved = solved_cube();

        while i < arr.len() && !wrong {
            wrong = wrong || arr[i] != solved[i];
            i = i + 1;
        }

        return !wrong;
    }

    pub fn new() -> Cube {
        return Cube {
            cube: solved_cube(), 
            solved: true
        };
    }

    fn modify_array(arr: [Color; 54], from_to: Vec<(usize,usize)>) -> [Color; 54] {
        let mut modified = arr;

        for ft in from_to {
            modified[ft.1] = arr[ft.0];
        }

        modified
    }

    fn get_turn_vector(tn: Turn) -> Vec<(usize, usize)>
    {
        let vec: Vec<(usize, usize)> = match tn {
            Turn::U => vec![(0,2),(1,5),(2,8),(3,1),(5,7),(6,0),(7,3),(8,6),(9,36),(10,37),(11,38),(18,9),(19,10),(20,11),(27,18),(28,19),(29,20),(36,27),(37,28),(38,29)],
            Turn::D => vec![(45,47),(47,53),(53,51),(51,45),(46,50),(50,52),(52,48),(48,46),(42,15),(43,16),(44,17),(15,24),(16,25),(17,26),(24,33),(25,34),(26,35),(33,42),(34,43),(35,44)],
            Turn::R => vec![(11,2),(14,5),(17,8),(47,11),(50,14),(53,17),(27,53),(30,50),(33,47),(2,33),(5,30),(8,27),(18,20),(20,26),(26,24),(24,18),(21,19),(19,23),(23,25),(25,21)],
            Turn::L => vec![(36,38),(38,44),(44,42),(42,36),(37,41),(41,43),(43,39),(39,37),(0,9),(3,12),(6,15),(9,45),(12,48),(15,51),(45,35),(48,32),(51,29),(35,0),(32,3),(29,6)],
            Turn::F => vec![(9,11),(11,17),(17,15),(15,9),(10,14),(14,16),(16,12),(12,10),(6,18),(7,21),(8,24),(18,47),(21,46),(24,45),(45,38),(46,41),(47,44),(38,8),(41,7),(44,6)],
            Turn::B => vec![(27,29),(29,35),(35,33),(33,27),(28,32),(32,34),(34,30),(30,28),(2,36),(1,39),(0,42),(36,51),(39,52),(42,53),(51,26),(52,23),(53,20),(26,2),(23,1),(20,0)],
            Turn::M => vec![(1,10),(4,13),(7,16),(10,46),(13,49),(16,52),(46,34),(49,31),(52,28),(34,1),(31,4),(28,7)],
            Turn::E => vec![(12,21),(13,22),(14,23),(21,30),(22,31),(23,32),(30,39),(31,40),(32,41),(39,12),(40,13),(41,14)],
            Turn::S => vec![(3,19),(4,22),(5,25),(19,50),(22,49),(25,48),(48,37),(49,40),(50,43),(37,5),(40,4),(43,3)],
            Turn::Up => vec![(2,0),(5,1),(8,2),(1,3),(7,5),(0,6),(3,7),(6,8),(36,9),(37,10),(38,11),(9,18),(10,19),(11,20),(18,27),(19,28),(20,29),(27,36),(28,37),(29,38)],
            Turn::Dp => vec![(47,45),(53,47),(51,53),(45,51),(50,46),(52,50),(48,52),(46,48),(15,42),(16,43),(17,44),(24,15),(25,16),(26,17),(33,24),(34,25),(35,26),(42,33),(43,34),(44,35)],        
            Turn::Rp => vec![(2,11),(5,14),(8,17),(11,47),(14,50),(17,53),(53,27),(50,30),(47,33),(33,2),(30,5),(27,8),(20,18),(26,20),(24,26),(18,24),(19,21),(23,19),(25,23),(21,25)],
            Turn::Lp => vec![(38,36),(44,38),(42,44),(36,42),(41,37),(43,41),(39,43),(37,39),(9,0),(12,3),(15,6),(45,9),(48,12),(51,15),(35,45),(32,48),(29,51),(0,35),(3,32),(6,29)],
            Turn::Fp => vec![(11,9),(17,11),(15,17),(9,15),(14,10),(16,14),(12,16),(10,12),(18,6),(21,7),(24,8),(47,18),(46,21),(45,24),(38,45),(41,46),(44,47),(8,38),(7,41),(6,44)],
            Turn::Bp => vec![(29,27),(35,29),(33,35),(27,33),(32,28),(34,32),(30,34),(28,30),(36,2),(39,1),(42,0),(51,36),(52,39),(53,42),(26,51),(23,52),(20,53),(2,26),(1,23),(0,20)],
            Turn::Mp => vec![(10,1),(13,4),(16,7),(46,10),(49,13),(52,16),(34,46),(31,49),(28,52),(1,34),(4,31),(7,28)],
            Turn::Ep => vec![(21,12),(22,13),(23,14),(30,21),(31,22),(32,23),(39,30),(40,31),(41,32),(12,39),(13,40),(14,41)],
            Turn::Sp => vec![(19,3),(22,4),(25,5),(50,19),(49,22),(48,25),(37,48),(40,49),(43,50),(5,37),(4,40),(3,43)],
            Turn::None => vec![],
        };

        vec
    }

    pub fn turn_dir(cb: Cube, tn: Turn) -> Cube {
        let modified = modify_array(cb.cube, get_turn_vector(tn));

        return Cube { cube: modified, solved: is_solved(modified) };
    }

    fn x_rot(c: String) -> String {
        let prt = c.chars().nth(0).unwrap();
        let prt_st = String::from(prt);

        let rt = String::from(match prt {
           'U' => "F",
           'D' => "B",
           'F' => "D",
           'B' => "U",
           'E' => "S'",
           'S' => "E",
           'u' => "f",
           'd' => "b",
           'f' => "d",
           'b' => "u",
           'y' => "z",
           'z' => "y'",
           _ => prt_st.as_str(),
        });
        
        let rpl = c.replace(prt_st.as_str(), rt.as_str());

        rpl.replace("''", "")
    }

    fn y_rot(c: String) -> String {
        let prt = c.chars().nth(0).unwrap();
        let prt_st = String::from(prt);

        let rt = String::from(match prt {
           'L' => "F",
           'R' => "B",
           'F' => "R",
           'B' => "L",
           'M' => "S",
           'S' => "M'",
           'l' => "f",
           'r' => "b",
           'f' => "r",
           'b' => "l",
           'x' => "z'",
           'z' => "x",
           _ => prt_st.as_str(),
        });
  
        let rpl = c.replace(prt_st.as_str(), rt.as_str());

        rpl.replace("''", "")
    }

    fn z_rot(c: String) -> String {
        let prt = c.chars().nth(0).unwrap();
        let prt_st = String::from(prt);

        let rt = String::from(match prt {
           'L' => "D",
           'R' => "U",
           'D' => "R",
           'U' => "L",
           'M' => "E",
           'E' => "M'",
           'l' => "d",
           'r' => "u",
           'd' => "r",
           'u' => "l",
           'x' => "y",
           'y' => "x'",
           _ => prt_st.as_str(),
        });
        
        let rpl = c.replace(prt_st.as_str(), rt.as_str());

        rpl.replace("''", "")
    }

    fn add_mod(num: i32, ad: i32, md: i32) -> i32 {
        let mut num = num + ad;

        if num >= md {
            num -= md;
        }
        else if num < 0 {
            num += md;
        }

        num
    }

    fn remove_rots(spl: Vec<String>) -> Vec<String> {
        let mut removed: Vec<String> = Vec::<String>::new();

        let mut xr = 0; let mut yr = 0; let mut zr = 0;

        for sp in spl {
            let fst = String::from(sp.chars().nth(0).unwrap());
            let counter = match sp.contains("'") {
                true => -1,
                false => 1,
            };

            if fst.as_str() == "x" {
                xr = add_mod(xr, counter, 4);
            }
            else if fst.as_str() == "y" {
                yr = add_mod(yr, counter, 4);
            }
            else if fst.as_str() == "z" {
                zr = add_mod(zr, counter, 4);
            }
            else {
                let mut chg = sp.clone();

                for _ in 0..xr {
                    chg = x_rot(chg);
                }

                for _ in 0..yr {
                    chg = y_rot(chg);
                }

                for _ in 0..zr {
                    chg = z_rot(chg);
                }

                removed.push(chg);
            }
        }

        removed
    }

    fn change_to_turn(turn: String, counter: bool, num: i32) -> Vec<Turn> {
        let mut num = num % 4;
        let mut cnt = counter;

        if num == 3 {
            cnt = !cnt;
            num = 1;
        }
        else if num == 2 {
            cnt = false;
        }
        else if num == 0 {
            return Vec::<Turn>::new();
        }

        let trns: Vec<Turn> = match cnt {
            // U, D, R, L, F, B, M, E, S, Up, Dp, Rp, Lp, Fp, Bp, Mp, Ep, Sp
            true => match turn.as_str() {
                "U" => vec![Turn::Up],
                "D" => vec![Turn::Dp],
                "R" => vec![Turn::Rp],
                "L" => vec![Turn::Lp],
                "F" => vec![Turn::Fp],
                "B" => vec![Turn::Bp],
                "M" => vec![Turn::Mp],
                "E" => vec![Turn::Ep],
                "S" => vec![Turn::Sp],
                "u" => vec![Turn::Up, Turn::E],
                "d" => vec![Turn::Dp, Turn::Ep],
                "r" => vec![Turn::Rp, Turn::Mp],
                "l" => vec![Turn::Lp, Turn::M],
                "f" => vec![Turn::Fp, Turn::S],
                "b" => vec![Turn::Bp, Turn::Sp],
                &_ => vec![],
            },
            false => match turn.as_str() {
                "U" => vec![Turn::U],
                "D" => vec![Turn::D],
                "R" => vec![Turn::R],
                "L" => vec![Turn::L],
                "F" => vec![Turn::F],
                "B" => vec![Turn::B],
                "M" => vec![Turn::M],
                "E" => vec![Turn::E],
                "S" => vec![Turn::S],
                "u" => vec![Turn::U, Turn::Ep],
                "d" => vec![Turn::D, Turn::E],
                "r" => vec![Turn::R, Turn::M],
                "l" => vec![Turn::L, Turn::Mp],
                "f" => vec![Turn::F, Turn::Sp],
                "b" => vec![Turn::B, Turn::S],
                &_ => vec![],
            },
        };

        let mut finl: Vec<Turn> = Vec::<Turn>::new();

        for t in trns {
            for _ in 0..num {
                finl.push(t);
            }
        }

        return finl;
    }

    pub fn turn_string(cb: Cube, tn: String) -> Cube {
        //const TURNS: [&str; 9] = ["U", "D", "R", "L", "F", "B", "M", "E", "S"];
        //const WIDES: [&str; 6] = ["u", "d", "r", "l", "f", "b"];
        //const ROTS: [&str; 3] = ["x", "y", "z"];

        let tn_split: Vec<String> = tn.split(' ').map(str::to_string).collect();
        let tn_split: Vec<String> = remove_rots(tn_split);

        let mut trns: Vec<Turn> = Vec::<Turn>::new();

        for t in tn_split {
            let trn = String::from(t.chars().nth(0).unwrap());
            let counter = t.contains("'");
            let num_st = match counter {
                true => t[2..].to_string(),
                false => t[1..].to_string(),
            };

            let mut num: i32 = 1;

            if !num_st.eq("") {
                num = num_st.parse().unwrap();
            }

            trns.extend(change_to_turn(trn, counter, num));
        }

        let rcube = do_turns(cb, trns);

        rcube
    }

    pub fn do_turns(cb: Cube, trns: Vec<Turn>) -> Cube {
        let mut rcube = cb.clone();

        for tr in trns {
            rcube = turn_dir(rcube, tr);
        }

        rcube
    }

    fn get_opposite(a: Turn) -> Turn {
        match a {
            Turn::U => Turn::Up,
            Turn::Up => Turn::U,
            Turn::D => Turn::Dp,
            Turn::Dp => Turn::D,
            Turn::R => Turn::Rp,
            Turn::Rp => Turn::R,
            Turn::L => Turn::Lp,
            Turn::Lp => Turn::L,
            Turn::F => Turn::Fp,
            Turn::Fp => Turn::F,
            Turn::B => Turn::Bp,
            Turn::Bp => Turn::B,
            Turn::M => Turn::Mp,
            Turn::Mp => Turn::M,
            Turn::E => Turn::Ep,
            Turn::Ep => Turn::E,
            Turn::S => Turn::Sp,
            Turn::Sp => Turn::S,
            Turn::None => Turn::None,
        }
    }

    fn is_cancelling(a: Turn, b: Turn) -> bool {
        a == get_opposite(b)
    }

    fn generate_scramble() -> String {
        //U, D, R, L, F, B, M, E, S, Up, Dp, Rp, Lp, Fp, Bp, Mp, Ep, Sp
        let mut rng = rand::thread_rng();
        let mut turns: Vec<Turn> = Vec::<Turn>::new();

        for _ in 0..100 {
            let current = match rng.gen_range(0..18) {
                1 => Turn::D,
                2 => Turn::R,
                3 => Turn::L, 
                4 => Turn::F,
                5 => Turn::B,
                6 => Turn::M,
                7 => Turn::E,
                8 => Turn::S,
                9 => Turn::Up,
                10 => Turn::Dp,
                11 => Turn::Rp,
                12 => Turn::Lp, 
                13 => Turn::Fp,
                14 => Turn::Bp,
                15 => Turn::Mp,
                16 => Turn::Ep,
                17 => Turn::Sp,
                _ => Turn::U
            };

            turns.push(current);
        }

        let mut prev: Turn = turns[0];
        let mut num = 1;

        let mut txt: Vec<String> = Vec::<String>::new(); 

        for i in 1..100 {
            if turns[i] == prev {
                num = num + 1;
            }
            else if is_cancelling(prev, turns[i]) {
                num = num - 1;
            }
            else {
                if num == 0 {
                    continue;
                }

                if num < 0 {
                    prev = get_opposite(prev);
                    num = -num; 
                }

                let newtxt: String = match num {
                    1 => format!("{}",prev),
                    _ => format!("{}{}",prev,num)
                };
                
                txt.push(newtxt);

                prev = turns[i];
                num = 1;
            }
        }

        let fnl = txt.clone().join(" ");
        fnl
    }

    pub fn scramble(cb: Cube) -> Cube {
        let scramble: String = generate_scramble();
        let cb = turn_string(cb, scramble);

        cb
    }

    pub fn find_best_moves(cb: Cube, c_t: Turn, depth: i32) -> Score {
        let current = count_points(cb);

        if current == 54 || depth == 0 {
            return Score { turns: Vec::<Turn>::new(), score: current };
        }

        const P_MOVES: [Turn; 18] = [ Turn::U, Turn::D, Turn::R, Turn::L, Turn::F, Turn::B, Turn::M, Turn::E, Turn::S, 
            Turn::Up, Turn::Dp, Turn::Rp, Turn::Lp, Turn::Fp, Turn::Bp, Turn::Mp, Turn::Ep, Turn::Sp ];

        let mut score = -1;
        let mut crt: Vec<Turn> = Vec::<Turn>::new();

        for i in 0..P_MOVES.len() {
            let p_cb = turn_dir(cb, P_MOVES[i]);

            let sc = find_best_moves(p_cb, P_MOVES[i], depth - 1);

            if sc.score > score {
                score = sc.score;
                crt = sc.turns;
            }
        }

        crt.push(c_t);

        return Score { turns: crt, score: score };
    }
}

fn print_vec<T : std::fmt::Display>(vec: Vec<T>) {
    print!("Vector ->");
    for v in vec {
        print!(" {}",v);
    }
    print!("\n");
}

fn main() {
    use cube::Turn;

    let mut cb = cube::scramble(cube::new());

    while !cube::is_solved(cb.cube) {
        let score = find_best_moves(cb, Turn::None, 5);
        let trns = score.turns.clone();

        println!("{}",score.score);
        print_vec(score.turns);

        cb = cube::do_turns(cb, trns);
    }
}
