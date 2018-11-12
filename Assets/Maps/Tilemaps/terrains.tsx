<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.2" tiledversion="1.2.0" name="terrains" tilewidth="128" tileheight="128" tilecount="20" columns="5">
 <image source="terrains@128x128.png" width="640" height="512"/>
 <terraintypes>
  <terrain name="Collider block" tile="0"/>
  <terrain name="Collider cross" tile="1"/>
  <terrain name="Ladder" tile="2"/>
 </terraintypes>
 <tile id="0" type="Collider_block" terrain="0,0,0,0">
  <objectgroup draworder="index">
   <object id="6" x="0" y="0" width="128" height="128"/>
  </objectgroup>
 </tile>
 <tile id="1" type="Collider_cross" terrain="1,1,1,1">
  <objectgroup draworder="index">
   <object id="1" x="0" y="0" width="128" height="128"/>
  </objectgroup>
 </tile>
 <tile id="2" type="Ladder" terrain="2,2,2,2"/>
</tileset>
