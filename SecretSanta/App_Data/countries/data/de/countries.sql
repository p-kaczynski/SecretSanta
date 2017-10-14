SET NAMES utf8;

DROP TABLE IF EXISTS `countries`;

CREATE TABLE `countries` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '',
  `alpha_2` char(2) NOT NULL DEFAULT '',
  `alpha_3` char(3) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

INSERT INTO `countries` (`id`, `name`, `alpha_2`, `alpha_3`) VALUES
(4,'Afghanistan','af','afg'),
(818,'Ägypten','eg','egy'),
(248,'Åland','ax','ala'),
(8,'Albanien','al','alb'),
(12,'Algerien','dz','dza'),
(16,'Amerikanisch-Samoa','as','asm'),
(850,'Amerikanische Jungferninseln','vi','vir'),
(20,'Andorra','ad','and'),
(24,'Angola','ao','ago'),
(660,'Anguilla','ai','aia'),
(10,'Antarktika (Sonderstatus durch Antarktis-Vertrag)','aq','ata'),
(28,'Antigua und Barbuda','ag','atg'),
(226,'Äquatorialguinea','gq','gnq'),
(32,'Argentinien','ar','arg'),
(51,'Armenien','am','arm'),
(533,'Aruba','aw','abw'),
(31,'Aserbaidschan','az','aze'),
(231,'Äthiopien','et','eth'),
(36,'Australien','au','aus'),
(44,'Bahamas','bs','bhs'),
(48,'Bahrain','bh','bhr'),
(50,'Bangladesch','bd','bgd'),
(52,'Barbados','bb','brb'),
(112,'Belarus (Weißrussland)','by','blr'),
(56,'Belgien','be','bel'),
(84,'Belize','bz','blz'),
(204,'Benin','bj','ben'),
(60,'Bermuda','bm','bmu'),
(64,'Bhutan','bt','btn'),
(68,'Bolivien','bo','bol'),
(535,'Bonaire, Sint Eustatius und Saba (Niederlande)','bq','bes'),
(70,'Bosnien und Herzegowina','ba','bih'),
(72,'Botswana','bw','bwa'),
(74,'Bouvetinsel','bv','bvt'),
(76,'Brasilien','br','bra'),
(92,'Britische Jungferninseln','vg','vgb'),
(86,'Britisches Territorium im Indischen Ozean','io','iot'),
(96,'Brunei Darussalam','bn','brn'),
(100,'Bulgarien','bg','bgr'),
(854,'Burkina Faso','bf','bfa'),
(108,'Burundi','bi','bdi'),
(152,'Chile','cl','chl'),
(156,'China, Volksrepublik','cn','chn'),
(184,'Cookinseln','ck','cok'),
(188,'Costa Rica','cr','cri'),
(384,'Côte d’Ivoire (Elfenbeinküste)','ci','civ'),
(531,'Curaçao','cw','cuw'),
(208,'Dänemark','dk','dnk'),
(276,'Deutschland','de','deu'),
(212,'Dominica','dm','dma'),
(214,'Dominikanische Republik','do','dom'),
(262,'Dschibuti','dj','dji'),
(218,'Ecuador','ec','ecu'),
(222,'El Salvador','sv','slv'),
(232,'Eritrea','er','eri'),
(233,'Estland','ee','est'),
(238,'Falklandinseln','fk','flk'),
(234,'Färöer','fo','fro'),
(242,'Fidschi','fj','fji'),
(246,'Finnland','fi','fin'),
(250,'Frankreich','fr','fra'),
(254,'Französisch-Guayana','gf','guf'),
(258,'Französisch-Polynesien','pf','pyf'),
(260,'Französische Süd- und Antarktisgebiete','tf','atf'),
(266,'Gabun','ga','gab'),
(270,'Gambia','gm','gmb'),
(268,'Georgien','ge','geo'),
(288,'Ghana','gh','gha'),
(292,'Gibraltar','gi','gib'),
(308,'Grenada','gd','grd'),
(300,'Griechenland','gr','grc'),
(304,'Grönland','gl','grl'),
(312,'Guadeloupe','gp','glp'),
(316,'Guam','gu','gum'),
(320,'Guatemala','gt','gtm'),
(831,'Guernsey (Kanalinsel)','gg','ggy'),
(324,'Guinea','gn','gin'),
(624,'Guinea-Bissau','gw','gnb'),
(328,'Guyana','gy','guy'),
(332,'Haiti','ht','hti'),
(334,'Heard und McDonaldinseln','hm','hmd'),
(340,'Honduras','hn','hnd'),
(344,'Hongkong','hk','hkg'),
(356,'Indien','in','ind'),
(360,'Indonesien','id','idn'),
(833,'Insel Man','im','imn'),
(368,'Irak','iq','irq'),
(364,'Iran, Islamische Republik','ir','irn'),
(372,'Irland','ie','irl'),
(352,'Island','is','isl'),
(376,'Israel','il','isr'),
(380,'Italien','it','ita'),
(388,'Jamaika','jm','jam'),
(392,'Japan','jp','jpn'),
(887,'Jemen','ye','yem'),
(832,'Jersey (Kanalinsel)','je','jey'),
(400,'Jordanien','jo','jor'),
(136,'Kaimaninseln','ky','cym'),
(116,'Kambodscha','kh','khm'),
(120,'Kamerun','cm','cmr'),
(124,'Kanada','ca','can'),
(132,'Kap Verde','cv','cpv'),
(398,'Kasachstan','kz','kaz'),
(634,'Katar','qa','qat'),
(404,'Kenia','ke','ken'),
(417,'Kirgisistan','kg','kgz'),
(296,'Kiribati','ki','kir'),
(166,'Kokosinseln','cc','cck'),
(170,'Kolumbien','co','col'),
(174,'Komoren','km','com'),
(180,'Kongo, Demokratische Republik (ehem. Zaire)','cd','cod'),
(178,'Kongo, Republik (ehem. K.-Brazzaville)','cg','cog'),
(408,'Korea, Demokratische Volksrepublik (Nordkorea)','kp','prk'),
(410,'Korea, Republik (Südkorea)','kr','kor'),
(191,'Kroatien','hr','hrv'),
(192,'Kuba','cu','cub'),
(414,'Kuwait','kw','kwt'),
(418,'Laos, Demokratische Volksrepublik','la','lao'),
(426,'Lesotho','ls','lso'),
(428,'Lettland','lv','lva'),
(422,'Libanon','lb','lbn'),
(430,'Liberia','lr','lbr'),
(434,'Libyen','ly','lby'),
(438,'Liechtenstein','li','lie'),
(440,'Litauen','lt','ltu'),
(442,'Luxemburg','lu','lux'),
(446,'Macau','mo','mac'),
(450,'Madagaskar','mg','mdg'),
(454,'Malawi','mw','mwi'),
(458,'Malaysia','my','mys'),
(462,'Malediven','mv','mdv'),
(466,'Mali','ml','mli'),
(470,'Malta','mt','mlt'),
(504,'Marokko','ma','mar'),
(584,'Marshallinseln','mh','mhl'),
(474,'Martinique','mq','mtq'),
(478,'Mauretanien','mr','mrt'),
(480,'Mauritius','mu','mus'),
(175,'Mayotte','yt','myt'),
(807,'Mazedonien','mk','mkd'),
(484,'Mexiko','mx','mex'),
(583,'Mikronesien','fm','fsm'),
(498,'Moldawien (Republik Moldau)','md','mda'),
(492,'Monaco','mc','mco'),
(496,'Mongolei','mn','mng'),
(499,'Montenegro','me','mne'),
(500,'Montserrat','ms','msr'),
(508,'Mosambik','mz','moz'),
(104,'Myanmar (Burma)','mm','mmr'),
(516,'Namibia','na','nam'),
(520,'Nauru','nr','nru'),
(524,'Nepal','np','npl'),
(540,'Neukaledonien','nc','ncl'),
(554,'Neuseeland','nz','nzl'),
(558,'Nicaragua','ni','nic'),
(528,'Niederlande','nl','nld'),
(562,'Niger','ne','ner'),
(566,'Nigeria','ng','nga'),
(570,'Niue','nu','niu'),
(580,'Nördliche Marianen','mp','mnp'),
(574,'Norfolkinsel','nf','nfk'),
(578,'Norwegen','no','nor'),
(512,'Oman','om','omn'),
(40,'Österreich','at','aut'),
(626,'Osttimor (Timor-Leste)','tl','tls'),
(586,'Pakistan','pk','pak'),
(275,'Staat Palästina','ps','pse'),
(585,'Palau','pw','plw'),
(591,'Panama','pa','pan'),
(598,'Papua-Neuguinea','pg','png'),
(600,'Paraguay','py','pry'),
(604,'Peru','pe','per'),
(608,'Philippinen','ph','phl'),
(612,'Pitcairninseln','pn','pcn'),
(616,'Polen','pl','pol'),
(620,'Portugal','pt','prt'),
(630,'Puerto Rico','pr','pri'),
(638,'Réunion','re','reu'),
(646,'Ruanda','rw','rwa'),
(642,'Rumänien','ro','rou'),
(643,'Russische Föderation','ru','rus'),
(90,'Salomonen','sb','slb'),
(652,'Saint-Barthélemy','bl','blm'),
(663,'Saint-Martin (franz. Teil)','mf','maf'),
(894,'Sambia','zm','zmb'),
(882,'Samoa','ws','wsm'),
(674,'San Marino','sm','smr'),
(678,'São Tomé und Príncipe','st','stp'),
(682,'Saudi-Arabien','sa','sau'),
(752,'Schweden','se','swe'),
(756,'Schweiz (Confoederatio Helvetica)','ch','che'),
(686,'Senegal','sn','sen'),
(688,'Serbien','rs','srb'),
(690,'Seychellen','sc','syc'),
(694,'Sierra Leone','sl','sle'),
(716,'Simbabwe','zw','zwe'),
(702,'Singapur','sg','sgp'),
(534,'Sint Maarten (niederl. Teil)','sx','sxm'),
(703,'Slowakei','sk','svk'),
(705,'Slowenien','si','svn'),
(706,'Somalia','so','som'),
(724,'Spanien','es','esp'),
(144,'Sri Lanka','lk','lka'),
(654,'St. Helena','sh','shn'),
(659,'St. Kitts und Nevis','kn','kna'),
(662,'St. Lucia','lc','lca'),
(666,'Saint-Pierre und Miquelon','pm','spm'),
(670,'St. Vincent und die Grenadinen','vc','vct'),
(710,'Südafrika','za','zaf'),
(729,'Sudan','sd','sdn'),
(239,'Südgeorgien und die Südlichen Sandwichinseln','gs','sgs'),
(728,'Südsudan','ss','ssd'),
(740,'Suriname','sr','sur'),
(744,'Svalbard und Jan Mayen','sj','sjm'),
(748,'Swasiland','sz','swz'),
(760,'Syrien, Arabische Republik','sy','syr'),
(762,'Tadschikistan','tj','tjk'),
(158,'Republik China (Taiwan)','tw','twn'),
(834,'Tansania, Vereinigte Republik','tz','tza'),
(764,'Thailand','th','tha'),
(768,'Togo','tg','tgo'),
(772,'Tokelau','tk','tkl'),
(776,'Tonga','to','ton'),
(780,'Trinidad und Tobago','tt','tto'),
(148,'Tschad','td','tcd'),
(203,'Tschechien','cz','cze'),
(788,'Tunesien','tn','tun'),
(792,'Türkei','tr','tur'),
(795,'Turkmenistan','tm','tkm'),
(796,'Turks- und Caicosinseln','tc','tca'),
(798,'Tuvalu','tv','tuv'),
(800,'Uganda','ug','uga'),
(804,'Ukraine','ua','ukr'),
(348,'Ungarn','hu','hun'),
(581,'United States Minor Outlying Islands','um','umi'),
(858,'Uruguay','uy','ury'),
(860,'Usbekistan','uz','uzb'),
(548,'Vanuatu','vu','vut'),
(336,'Vatikanstadt','va','vat'),
(862,'Venezuela','ve','ven'),
(784,'Vereinigte Arabische Emirate','ae','are'),
(840,'Vereinigte Staaten von Amerika','us','usa'),
(826,'Vereinigtes Königreich Großbritannien und Nordirland','gb','gbr'),
(704,'Vietnam','vn','vnm'),
(876,'Wallis und Futuna','wf','wlf'),
(162,'Weihnachtsinsel','cx','cxr'),
(732,'Westsahara','eh','esh'),
(140,'Zentralafrikanische Republik','cf','caf'),
(196,'Zypern','cy','cyp')