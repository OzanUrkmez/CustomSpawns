<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output omit-xml-declaration="yes"/>

    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="//CraftingTemplate[@id='TwoHandedMace']//UsablePiece[not(following-sibling::UsablePiece)]">
		<xsl:copy-of select="."/> 
        <UsablePiece piece_id="cs_maul_tip_1"/>
        <UsablePiece piece_id="cs_morningstar_shaft"/>
        <UsablePiece piece_id="cs_morningstar_head"/>
        <UsablePiece piece_id="cs_wooden_morningstar_head"/>
        <UsablePiece piece_id="cs_wooden_morningstar_head_2"/>
        <UsablePiece piece_id="cs_wooden_morningstar_head_3"/>
        <UsablePiece piece_id="cs_s_morningstar_shaft"/>
        <UsablePiece piece_id="cs_ls_morningstar_shaft"/>
        <UsablePiece piece_id="cs_club_handle"/>
        <UsablePiece piece_id="cs_no_head"/>
        <!-- Vanilla pieces not defined in TwoHandedMace crafting template as from e1.8.0 -->
        <UsablePiece piece_id="mace_head_7" />
        <UsablePiece piece_id="mace_handle_15" />
    </xsl:template>

    <xsl:template match="//CraftingTemplate[@id='Javelin']//UsablePiece[not(following-sibling::UsablePiece)]">
        <xsl:copy-of select="."/>
        <UsablePiece piece_id="cs_bamboo_spear_handle"/>
    </xsl:template>

    <xsl:template match="//CraftingTemplate[@id='Pike']//UsablePiece[not(following-sibling::UsablePiece)]">
        <xsl:copy-of select="."/>
        <UsablePiece piece_id="cs_barbarian_pike_shaft"/>
        <!-- Vanilla pieces not defined in Pike crafting template as from e1.8.0 -->
        <UsablePiece piece_id="spear_handle_3" />
    </xsl:template>

    <xsl:template match="//CraftingTemplate[@id='TwoHandedAxe']//UsablePiece[not(following-sibling::UsablePiece)]">
        <xsl:copy-of select="."/>
        <UsablePiece piece_id="cs_pick_head"/>
        <!-- Vanilla pieces not defined in TwoHandedAxe crafting template as from e1.8.0 -->
        <UsablePiece piece_id="axe_craft_26_head" />
    </xsl:template>

    <xsl:template match="//CraftingTemplate[@id='TwoHandedPolearm']//UsablePiece[not(following-sibling::UsablePiece)]">
        <xsl:copy-of select="."/>
        <UsablePiece piece_id="cs_halberd_head"/>
        <UsablePiece piece_id="cs_halberd_handle"/>
        <UsablePiece piece_id="cs_harpoon_shaft"/>
        <UsablePiece piece_id="cs_blunt_spear_head"/>
    </xsl:template>
</xsl:stylesheet>
