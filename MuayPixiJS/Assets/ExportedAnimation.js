const BlackFighter_SR = await Assets.load("assets/Fighter2.json");const BlackFighter = new Spine(BlackFighter_SR.spineData);BlackFighter.x = 340;BlackFighter.y = 730;BlackFighter.scale.set(0.6,0.6);const WhiteFighter_SR = await Assets.load("assets/Fighter.json");const WhiteFighter = new Spine(WhiteFighter_SR.spineData);WhiteFighter.x = 560;WhiteFighter.y = 730;WhiteFighter.scale.set(-0.6,0.6);const HitFx_SR = await Assets.load("assets/Hit.json");const HitFx = new Spine(HitFx_SR.spineData);HitFx.x = 450;HitFx.y = 541;HitFx.scale.set(1,1);